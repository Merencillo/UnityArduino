using UnityEngine;
using System.Collections.Generic;
using System;
using System.Threading;

#if UNITY_STANDALONE
using System.IO;
using System.IO.Ports;
#endif


namespace Ardunity
{
	[AddComponentMenu("ARDUnity/CommSocket/CommSerial")]
    [HelpURL("https://sites.google.com/site/ardunitydoc/references/commsocket/commserial")]
	public class CommSerial : CommSocket
	{
		public int baudrate = 115200;
        public bool dtrReset = true;

        private bool _threadOnOpen = false;
        private bool _threadOnOpenFailed = false;
        private Thread _openThread;

#if UNITY_STANDALONE
        private SerialPort _serialPort;        
#endif

        protected override void Awake()
		{
            base.Awake();
            
#if UNITY_STANDALONE
			_serialPort = new SerialPort();			
			_serialPort.DataBits = 8;
			_serialPort.Parity = Parity.None;
			_serialPort.StopBits = StopBits.One;
			_serialPort.ReadTimeout = 1; // since on windows we *cannot* have a separate read thread            
			_serialPort.WriteTimeout = 1000;
            _serialPort.Handshake = Handshake.None;
			_serialPort.RtsEnable = false;
#endif
        }

        void Update()
        {
            if (_threadOnOpen)
            {
                OnOpen.Invoke();
                _threadOnOpen = false;
            }

            if (_threadOnOpenFailed)
            {
                OnOpenFailed.Invoke();
                _threadOnOpenFailed = false;
            }
        }

        #region Override
        public override void Open()
        {
            if (IsOpen)
                return;

            _openThread = new Thread(openThread);
            _openThread.Start();
        }

        public override void Close()
        {
            if (!IsOpen)
                return;

            ErrorClose();
            OnClose.Invoke();
        }

        protected override void ErrorClose()
        {
#if UNITY_STANDALONE
            try
            {
				_serialPort.Close();
			}
			catch(Exception)
			{
			}
#endif
        }

        public override bool IsOpen
        {
            get
            {
#if UNITY_STANDALONE
                if (_serialPort == null)
                    return false;

                return _serialPort.IsOpen;
#else
                return false;
#endif
            }
        }

        public override void StartSearch()
        {
            foundDevices.Clear();
            OnStartSearch.Invoke();            

#if UNITY_STANDALONE
#if UNITY_EDITOR_WIN
            WindowPortSearch();
#elif UNITY_EDITOR_OSX
            OsxPortSearch();
#elif UNITY_STANDALONE_WIN
            WindowPortSearch();
#elif UNITY_STANDALONE_OSX
            OsxPortSearch();
#else
            Debug.Log("CommSerial: Not Supported Platform!");
#endif
#endif

            OnStopSearch.Invoke();
        }
        
#if UNITY_STANDALONE
        private void WindowPortSearch()
        {
            string[] ports = SerialPort.GetPortNames();
            foreach (string port in ports)
            {
                CommDevice foundDevice = new CommDevice();
                foundDevice.name = port;
                foundDevice.address = "//./" + port;
                foundDevices.Add(foundDevice);
                OnFoundDevice.Invoke(foundDevice);
            }
        }
        
        private void OsxPortSearch()
        {
            string prefix = "/dev/";
            string[] ports = Directory.GetFiles(prefix, "*.*");
            foreach (string port in ports)
            {
                if (port.StartsWith("/dev/cu."))
                {
                    CommDevice foundDevice = new CommDevice();
                    foundDevice.name = port.Substring(prefix.Length);
                    foundDevice.address = port;
                    foundDevices.Add(foundDevice);
                    OnFoundDevice.Invoke(foundDevice);
                }
            }
        }
#endif

        public override void Write(byte[] data, bool getCompleted = false)
        {
            if (data == null)
                return;
            if (data.Length == 0)
                return;

#if UNITY_STANDALONE
            try
            {
                _serialPort.Write(data, 0, data.Length);
                if(getCompleted)
                    OnWriteCompleted.Invoke();
            }
            catch (Exception)
            {
                ErrorClose();
                OnErrorClosed.Invoke();
            }
#endif
        }

        public override byte[] Read()
        {
#if UNITY_STANDALONE
            List<byte> bytes = new List<byte>();

            while (true)
            {
                try
                {
                    bytes.Add((byte)_serialPort.ReadByte());
                }
                catch (TimeoutException)
                {
                    break;
                }
                catch (Exception)
                {
                    ErrorClose();
                    OnErrorClosed.Invoke();
                    return null;
                }
            }

            if (bytes.Count == 0)
                return null;
            else
                return bytes.ToArray();
#else
            return null;
#endif
        }
        #endregion

        private void openThread()
        {
#if UNITY_STANDALONE
            try
            {
                _serialPort.PortName = device.address;
                _serialPort.BaudRate = baudrate;
                _serialPort.DtrEnable = dtrReset;
                _serialPort.Open();
                _threadOnOpen = true;
            }
            catch (Exception)
            {
                _threadOnOpenFailed = true;
            }
#else
            _threadOnOpenFailed = true;
#endif
            _openThread.Abort();
            return;
        }
        
        protected override void AddNode(List<Node> nodes)
        {
			base.AddNode(nodes);
            
            nodes.Add(new Node("baudrate", "", null, NodeType.None, "Serial Baudrate Speed"));
        }
        
        protected override void UpdateNode(Node node)
        {
            if(node.name.Equals("baudrate"))
            {
                node.updated = true;
                node.text = string.Format("{0:d} bps", baudrate);
                return;
            }
            
            base.UpdateNode(node);
        }
    }
}
