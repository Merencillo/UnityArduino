using UnityEngine;
using System.Collections.Generic;

using UINT8 = System.Byte;


namespace Ardunity
{
	[AddComponentMenu("ARDUnity/Controller/Motor/GenericServo")]
    [HelpURL("https://sites.google.com/site/ardunitydoc/references/controller/genericservo")]
	public class GenericServo : ArdunityController, IWireOutput<float>
	{
		public int pin;
		public bool smooth = false;
		
		[SerializeField]
		private int _calibratedAngle = 0;
        [SerializeField]
		private int _minAngle = -90;
        [SerializeField]
		private int _maxAngle = 90;
		[SerializeField]
		private int _angle = 0;
		
		protected override void Awake()
		{
			base.Awake();
			
			enableUpdate = false; // only output.
		}
		
		protected override void OnPush()
		{
			Push((UINT8)Mathf.Clamp(_angle + _calibratedAngle + 90, 0, 180));
		}
		
		public override string[] GetCodeIncludes()
		{
			List<string> includes = new List<string>();
			includes.Add("#include <Servo.h>");
			return includes.ToArray();
		}
		
		public override string GetCodeDeclaration()
		{
			string declaration = string.Format("{0} {1}({2:d}, {3:d}, ", this.GetType().Name, GetCodeVariable(), id, pin);
			if(smooth)
				declaration += "true);";
			else
				declaration += "false);";
			
			return declaration;
		}
		
		public override string GetCodeVariable()
		{
			return string.Format("servo{0:d}", id);
		}
		
		public int calibratedAngle
		{
			get
			{
				return _calibratedAngle;
			}
			set
			{
				value = Mathf.Clamp(value, -45, 45);
				if(_calibratedAngle != value)
				{
					_calibratedAngle = value;
					SetDirty();
				}
			}
		}
        
        public int minAngle
		{
			get
			{
				return _minAngle;
			}
			set
			{
				_minAngle = Mathf.Clamp(value, -90, _maxAngle);
                angle = _angle;
			}
		}
        
        public int maxAngle
		{
			get
			{
				return _maxAngle;
			}
			set
			{
				_maxAngle = Mathf.Clamp(value, _minAngle, 90);
                angle = _angle;
			}
		}
		
		public int angle
		{
			get
			{
				return _angle;
			}
			set
			{
				value = Mathf.Clamp(value, _minAngle, _maxAngle);
				if(_angle != value)
				{
					_angle = value;
					SetDirty();
				}
			}
		}
		
		float IWireOutput<float>.output
        {
			get
			{
				return (float)angle;
			}
            set
            {
				if(value > 180f)
					value -= 360f;
				else if(value < -180f)
					value += 360f;
				angle = (int)value;
            }
		}
		
		protected override void AddNode(List<Node> nodes)
        {
			base.AddNode(nodes);
			
            nodes.Add(new Node("pin", "", null, NodeType.None, "Arduino Digital Pin"));
            nodes.Add(new Node("angle", "Angle", typeof(IWireOutput<float>), NodeType.WireTo, "Output<float>"));
        }
        
        protected override void UpdateNode(Node node)
        {
            if(node.name.Equals("pin"))
            {
				node.updated = true;
                node.text = string.Format("Pin: {0:d}", pin);
                return;
            }
			else if(node.name.Equals("angle"))
            {
				node.updated = true;
                return;
            }
            
            base.UpdateNode(node);
        }
	}
}
