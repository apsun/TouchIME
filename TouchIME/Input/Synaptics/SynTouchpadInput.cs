using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SYNCTRLLib;

namespace TouchIME.Input.Synaptics
{
    /// <summary>
    /// Managed wrapper for the Synaptics touchpad API.
    /// </summary>
    public sealed class SynTouchpadInput : IRawTouchInput
    {
        private readonly SynAPICtrl _api = new SynAPICtrl();
        private readonly SynDeviceCtrl _device = new SynDeviceCtrl();
        private readonly SynPacketCtrl _packet = new SynPacketCtrl();
        private readonly int _lowX;
        private readonly int _highY;
        private bool _acquired;
        private bool _touching;
        private bool _disposed;
        
        public event EventHandler TouchStarted;
        public event EventHandler<RawTouchEventArgs> TouchMoved;
        public event EventHandler TouchEnded;
        
        public Rectangle TouchArea
        {
            get
            {
                EnsureNotDisposed();
                int minX = NormalizeX(_device.GetLongProperty(SynDeviceProperty.SP_XLoSensor));
                int maxX = NormalizeX(_device.GetLongProperty(SynDeviceProperty.SP_XHiSensor));
                int minY = NormalizeY(_device.GetLongProperty(SynDeviceProperty.SP_YHiSensor));
                int maxY = NormalizeY(_device.GetLongProperty(SynDeviceProperty.SP_YLoSensor));
                return Rectangle.FromLTRB(minX, minY, maxX, maxY);
            }
        }

        public bool TouchEnabled
        {
            get
            {
                EnsureNotDisposed();
                return _device.GetLongProperty(SynDeviceProperty.SP_DisableState) == 0;
            }
            set
            {
                EnsureNotDisposed();
                _device.SetLongProperty(SynDeviceProperty.SP_DisableState, value ? 0 : 1);
            }
        }

        /// <summary>
        /// Initializes the touch input manager.
        /// </summary>
        /// <exception cref="SynDriverException">
        /// Thrown if the Synaptics driver is not installed or is outdated.
        /// </exception>
        /// <exception cref="SynDeviceNotFoundException">
        /// Thrown if a touchpad could not be found.
        /// </exception>
        public SynTouchpadInput()
        {
            try
            {
                _api.Initialize();
            }
            catch (COMException ex) when (ex.ErrorCode == (int)SynError.Fail)
            {
                throw new SynDriverException();
            }
            int handle = _api.FindDevice(SynConnectionType.SE_ConnectionAny, SynDeviceType.SE_DeviceTouchPad, -1);
            if (handle < 0) throw new SynDeviceNotFoundException();
            _device.Select(handle);
            _device.Activate();
            _lowX = _device.GetLongProperty(SynDeviceProperty.SP_XLoSensor);
            _highY = _device.GetLongProperty(SynDeviceProperty.SP_YHiSensor);
        }
        
        public void StartTouchCapture()
        {
            EnsureNotDisposed();
            if (_acquired) return;
            try
            {
                _device.Acquire(0);
            }
            catch (UnauthorizedAccessException ex)
            {
                // From Synaptics SDK documentation:
                //
                // The device cannot be acquired. The most likely cause is that
                // another program has previously acquired the device.
                throw new TouchCaptureException(ex);
            }
            _device.OnPacket += OnDevicePacket;
            _acquired = true;
        }
        
        public void StopTouchCapture()
        {
            EnsureNotDisposed();
            if (!_acquired) return;
            _device.OnPacket -= OnDevicePacket;
            _device.Unacquire();
            _acquired = false;
            if (_touching)
            {
                TouchEnded?.Invoke(this, EventArgs.Empty);
                _touching = false;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int NormalizeX(int rawX)
        {
            return rawX - _lowX;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int NormalizeY(int rawY)
        {
            return _highY - rawY;
        }

        private bool LoadPacket()
        {
            // if (_disposed) return false;
            try
            {
                return _device.LoadPacket(_packet) == 0;
            }
            catch (COMException ex) when (ex.ErrorCode == (int)SynError.Sequence)
            {
                // From Synaptics SDK documentation:
                //
                // Packet data was correctly exported but the packet's sequence
                // number was not sequential with the previously exported packet.
                Debug.WriteLine("Packet sequence out of order");
                return true;
            }
            catch (COMException ex) when (ex.ErrorCode == (int)SynError.Fail)
            {
                // From Synaptics SDK documentation:
                //
                // The device packet queue is empty.
                Debug.WriteLine("Packet queue is empty");
                return false;
            }
        }

        private void OnDevicePacket()
        {
            // WARNING: This is called on a separate thread from the thread
            // that initialized the object!
            if (!LoadPacket()) return;
            int fingerState = _packet.FingerState;
            bool touching = (fingerState & (int)SynFingerFlags.SF_FingerPresent) != 0;
            if (touching != _touching)
            {
                _touching = touching;
                EventHandler handler = touching ? TouchStarted : TouchEnded;
                handler?.Invoke(this, EventArgs.Empty);
            }
            if (touching)
            {
                int x = NormalizeX(_packet.XRaw);
                int y = NormalizeY(_packet.YRaw);
                TouchMoved?.Invoke(this, new RawTouchEventArgs(x, y));
            }
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
            _device.Deactivate();
            _disposed = true;
        }

        ~SynTouchpadInput()
        {
            if (!_disposed)
            {
                Debug.WriteLine("SynTouchpadInput finalized without Dispose!");
            }
        }
    }
}
