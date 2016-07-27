using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Ink;
using TouchIME.Input;

namespace TouchIME.Recognition
{
    /// <summary>
    /// Converts touch strokes to text.
    /// </summary>
    public sealed class StrokeRecognizer : IDisposable
    {
        private readonly Recognizers _recognizers = new Recognizers();
        private readonly Ink _ink;
        private readonly Strokes _strokes;
        private RecognizerContext _recognizerContext;
        private bool _disposed;

        /// <summary>
        /// Initializes the stroke recognizer. You must set a recognizer
        /// implementation with <see cref="SetEngine"/> before
        /// calling <see cref="Recognize(int)"/>. 
        /// </summary>
        public StrokeRecognizer()
        {
            _ink = new Ink();
            _strokes = _ink.Strokes;
        }
        
        /// <summary>
        /// Gets all available recognizer engines installed on the computer.
        /// </summary>
        public StrokeRecognizerEngine[] GetAllEngines()
        {
            return _recognizers
                .Cast<Recognizer>()
                .Where(x => x.Languages.Length > 0)
                .Select(x => new StrokeRecognizerEngine(x.Name, x.Id))
                .ToArray();
        }

        /// <summary>
        /// Gets the default recognizer engine for the current language,
        /// or null if none is available.
        /// </summary>
        public StrokeRecognizerEngine GetDefaultEngine()
        {
            Recognizer engineImpl = _recognizers.GetDefaultRecognizer();
            if (engineImpl == null) return null;
            return new StrokeRecognizerEngine(engineImpl.Name, engineImpl.Id);
        }

        /// <summary>
        /// Sets the recognizer engine used to convert strokes to text.
        /// </summary>
        /// <param name="engineId">The GUID of the recognizer engine.</param>
        /// <exception cref="ArgumentException">
        /// Thrown if the engine is invalid or does not exist.
        /// </exception>
        public void SetEngine(Guid engineId)
        {
            EnsureNotDisposed();
            _recognizerContext?.Dispose();
            Recognizer engineImpl = _recognizers
                .Cast<Recognizer>()
                .FirstOrDefault(x => x.Id == engineId);
            if (engineImpl == null)
            {
                throw new ArgumentException("Engine not found: " + engineId);
            }
            _recognizerContext = engineImpl.CreateRecognizerContext();
        }

        /// <summary>
        /// Adds a new stroke to be recognized. Call <see cref="Recognize"/>
        /// to convert the strokes to text.
        /// </summary>
        /// <param name="stroke">The stroke to add.</param>
        public void AddStroke(TouchStroke stroke)
        {
            EnsureNotDisposed();
            _strokes.Add(_ink.CreateStroke(stroke.GetPoints()));
        }

        /// <summary>
        /// Clears all strokes added with <see cref="AddStroke(TouchStroke)"/>. 
        /// </summary>
        public void ClearStrokes()
        {
            EnsureNotDisposed();
            _strokes.Clear();
        }

        /// <summary>
        /// Performs stroke-to-text recognition on all strokes added with
        /// <see cref="AddStroke(TouchStroke)"/>. An empty list is returned
        /// if no matches were found. You may modify the returned list. 
        /// </summary>
        /// <param name="maxResults">Maximum number of results to return.</param>
        /// <exception cref="InvalidOperationException">
        /// Thrown if no recognizer implementation has been set. 
        /// </exception>
        /// <exception cref="StrokeRecognitionException">
        /// Thrown if stroke recognition failed.
        /// </exception>
        public List<string> Recognize(int maxResults = 10)
        {
            EnsureNotDisposed();
            if (_recognizerContext == null)
            {
                throw new InvalidOperationException("Recognizer not set");
            }
            if (_strokes.Count == 0)
            {
                return new List<string>();
            }
            _recognizerContext.Strokes = _strokes;
            RecognitionStatus status;
            RecognitionResult recognizeResult = _recognizerContext.Recognize(out status);
            if (status != RecognitionStatus.NoError)
            {
                throw new StrokeRecognitionException("Recognition status: " + status);
            }
            RecognitionAlternates alternates;
            try
            {
                alternates = recognizeResult.GetAlternatesFromSelection(0, -1, maxResults);
            }
            catch (Exception ex)
            {
                throw new StrokeRecognitionException("Failed to get alternates", ex);
            }
            List<string> results = new List<string>(alternates.Count);
            foreach (RecognitionAlternate alternate in alternates)
            {
                results.Add(alternate.ToString());
            }
            return results;
        }

        private void EnsureNotDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
        }

        public void Dispose()
        {
            if (_disposed) return;
            _recognizerContext.Dispose();
            _strokes.Dispose();
            _ink.Dispose();
            _disposed = true;
        }

        ~StrokeRecognizer()
        {
            if (!_disposed)
            {
                Debug.WriteLine("StrokeRecognizer finalized without Dispose!");
            }
        }
    }
}
