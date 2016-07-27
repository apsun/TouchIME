using System;

namespace TouchIME.Recognition
{
    /// <summary>
    /// Represents a stroke recognizer engine used to convert
    /// strokes to text.
    /// </summary>
    public class StrokeRecognizerEngine : IEquatable<StrokeRecognizerEngine>
    {
        /// <summary>
        /// Gets the user-friendly name of the engine.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the unique identifier of the engine.
        /// </summary>
        public Guid Id { get; }

        internal StrokeRecognizerEngine(string name, Guid id)
        {
            Name = name;
            Id = id;
        }

        public bool Equals(StrokeRecognizerEngine other)
        {
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            return obj is StrokeRecognizerEngine && Equals((StrokeRecognizerEngine)obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
