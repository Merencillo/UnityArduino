using UnityEngine;
using System.Collections.Generic;


namespace Ardunity
{
	[AddComponentMenu("ARDUnity/Bridge/Output/ColorOutput")]
    [HelpURL("https://sites.google.com/site/ardunitydoc/references/bridge/coloroutput")]
	public class ColorOutput : ArdunityBridge, IWireOutput<Color>
	{
		[SerializeField]
		private Color _color;
        
		private IWireOutput<float> _analogRed;
		private IWireOutput<float> _analogBlue;
		private IWireOutput<float> _analogGreen;
        private IWireOutput<bool> _digitalRed;
		private IWireOutput<bool> _digitalBlue;
		private IWireOutput<bool> _digitalGreen;
        
        #region MonoBehaviour
		// Use this for initialization
		void Start ()
		{
		
		}
		
		// Update is called once per frame
		void Update ()
		{
		}
        #endregion
		
		public Color color
		{
			get
			{
				return _color;
			}
			set
			{
				if(_color != value)
				{
					_color = value;
					
					if(_analogRed != null)
						_analogRed.output = _color.r;
					if(_analogBlue != null)
						_analogBlue.output = _color.b;
					if(_analogGreen != null)
						_analogGreen.output = _color.g;
                    
                    if(_digitalRed != null)
					{
						if(_color.r > 0.5f)
							_digitalRed.output = true;
						else
							_digitalRed.output = false;
					}
					if(_digitalBlue != null)
					{
						if(_color.b > 0.5f)
							_digitalBlue.output = true;
						else
							_digitalBlue.output = false;
					}
					if(_digitalGreen != null)
					{
						if(_color.g > 0.5f)
							_digitalGreen.output = true;
						else
							_digitalGreen.output = false;
					}
				}
			}
		}

        #region Wire Editor
        Color IWireOutput<Color>.output
        {
			get
			{
				return color;
			}
            set
            {				
                color = value;
            }
        }
		
		protected override void AddNode(List<Node> nodes)
        {
			base.AddNode(nodes);
			
            nodes.Add(new Node("analogRed", "Red(analog)", typeof(IWireOutput<float>), NodeType.WireFrom, "Output<float>"));
			nodes.Add(new Node("analogGreen", "Green(analog)", typeof(IWireOutput<float>), NodeType.WireFrom, "Output<float>"));
			nodes.Add(new Node("analogBlue", "Blue(analog)", typeof(IWireOutput<float>), NodeType.WireFrom, "Output<float>"));
            nodes.Add(new Node("digitalRed", "Red(digital)", typeof(IWireOutput<bool>), NodeType.WireFrom, "Output<bool>"));
			nodes.Add(new Node("digitalGreen", "Green(digital)", typeof(IWireOutput<bool>), NodeType.WireFrom, "Output<bool>"));
			nodes.Add(new Node("digitalBlue", "Blue(digital)", typeof(IWireOutput<bool>), NodeType.WireFrom, "Output<bool>"));
			nodes.Add(new Node("outputColor", "Color", typeof(IWireOutput<Color>), NodeType.WireTo, "Output<Color>"));
        }
        
        protected override void UpdateNode(Node node)
        {
            if(node.name.Equals("analogRed"))
            {
                node.updated = true;
                if(node.objectTarget == null && _analogRed == null)
                    return;
                
                if(node.objectTarget != null)
                {
                    if(node.objectTarget.Equals(_analogRed))
                        return;
                }
                
                _analogRed = node.objectTarget as IWireOutput<float>;
                if(_analogRed == null)
                    node.objectTarget = null;
                
                return;
            }
            else if(node.name.Equals("analogGreen"))
            {
                node.updated = true;
                if(node.objectTarget == null && _analogGreen == null)
                    return;
                
                if(node.objectTarget != null)
                {
                    if(node.objectTarget.Equals(_analogGreen))
                        return;
                }

                _analogGreen = node.objectTarget as IWireOutput<float>;
                if(_analogGreen == null)
                    node.objectTarget = null;
                
                return;
            }
            else if(node.name.Equals("analogBlue"))
            {
                node.updated = true;
                if(node.objectTarget == null && _analogBlue == null)
                    return;
                
                if(node.objectTarget != null)
                {
                    if(node.objectTarget.Equals(_analogBlue))
                        return;
                }
                
                _analogBlue = node.objectTarget as IWireOutput<float>;
                if(_analogBlue == null)
                    node.objectTarget = null;
                
                return;
            }
            else if(node.name.Equals("digitalRed"))
            {
                node.updated = true;
                if(node.objectTarget == null && _digitalRed == null)
                    return;
                
                if(node.objectTarget != null)
                {
                    if(node.objectTarget.Equals(_digitalRed))
                        return;
                }
                
                _digitalRed = node.objectTarget as IWireOutput<bool>;
                if(_digitalRed == null)
                    node.objectTarget = null;
                
                return;
            }
            else if(node.name.Equals("digitalGreen"))
            {
                node.updated = true;
                if(node.objectTarget == null && _digitalGreen == null)
                    return;
                
                if(node.objectTarget != null)
                {
                    if(node.objectTarget.Equals(_digitalGreen))
                        return;
                }
                
                _digitalGreen = node.objectTarget as IWireOutput<bool>;
                if(_digitalGreen == null)
                    node.objectTarget = null;
                
                return;
            }
            else if(node.name.Equals("digitalBlue"))
            {
                node.updated = true;
                if(node.objectTarget == null && _digitalBlue == null)
                    return;
                
                if(node.objectTarget != null)
                {
                    if(node.objectTarget.Equals(_digitalBlue))
                        return;
                }
                
                _digitalBlue = node.objectTarget as IWireOutput<bool>;
                if(_digitalBlue == null)
                    node.objectTarget = null;
                
                return;
            }
            else if(node.name.Equals("outputColor"))
            {
                node.updated = true;
                return;
            }
            
            base.UpdateNode(node);
        }
        #endregion
	}
}
