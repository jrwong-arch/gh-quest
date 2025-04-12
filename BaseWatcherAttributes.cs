using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using GH_IO.Serialization;

using Grasshopper;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Attributes;
using Rhino.Geometry;

namespace gh_quest
{
    public class BaseWatcherAttributes : GH_ComponentAttributes
    {

        RectangleF _TitleBounds = new RectangleF();
        RectangleF _ButtonBounds = new RectangleF();
        Action _ButtonClickAction;
        string _ButtonText = "Launch";
        bool _MouseDown = false;
        bool _MouseHover = false;


        public BaseWatcherAttributes(IGH_Component component, Action buttonAction) : base(component)
        {
            _ButtonClickAction = buttonAction;
        }



        protected override void Layout()
        {
            base.Layout();

            float componentWidth = 100;
            float componentHeight = 50;
            Bounds = new RectangleF(Bounds.X - componentWidth/2, Bounds.Y, componentWidth, componentHeight);

            float edgeOffset = 3.0f;
            float buttonHeight = 50.0f;
            _TitleBounds = new RectangleF(Bounds.X + edgeOffset, Bounds.Top + edgeOffset, Bounds.Width - 2 * edgeOffset, buttonHeight);
            _ButtonBounds = new RectangleF(Bounds.X + edgeOffset, Bounds.Bottom + edgeOffset, Bounds.Width - 2 * edgeOffset, buttonHeight);
            Bounds = new RectangleF(Bounds.X, Bounds.Y, Bounds.Width, Bounds.Height + buttonHeight + (2 * edgeOffset));
        }


        protected override void Render(GH_Canvas canvas, Graphics graphics, GH_CanvasChannel channel)
        {
            //base.Render(canvas, graphics, channel);

            if (channel == GH_CanvasChannel.Objects)
            {

                var capsule = GH_Capsule.CreateCapsule(Bounds, GH_Palette.Pink);
                capsule.Render(graphics, Selected, Owner.Locked, false);
                capsule.Dispose();

                //Colors and Fonts
                Font buttonFont = new Font(GH_FontServer.Standard.FontFamily, GH_FontServer.Standard.Size / GH_GraphicsUtil.UiScale, FontStyle.Regular);

                Font titleFont = new System.Drawing.Font(GH_FontServer.Large.Name, 12, FontStyle.Bold);
                titleFont = new Font(titleFont.FontFamily, titleFont.Size / GH_GraphicsUtil.UiScale, FontStyle.Bold);

                Brush normalColor = new SolidBrush(Color.FromArgb(255,60,60,60));
                Brush hoverColor = new SolidBrush(Color.FromArgb(255,120,120,120));
                Brush clickedColor = new SolidBrush(Color.FromArgb(255,180,180,180));

                Color edgeColor = Color.Black;
                Color edgeHover = Color.Gray;
                Color edgeClick = Color.White;


                //Render Title Text
                graphics.DrawString("GH Quest", titleFont, new SolidBrush(Color.Black), _TitleBounds, GH_TextRenderingConstants.CenterCenter);


                //Render Button
                GraphicsPath button = RoundedRect(_ButtonBounds, 2);
                Brush buttonColor = _MouseHover ? hoverColor : normalColor;
                graphics.FillPath(_MouseDown ? clickedColor : buttonColor, button);

                //Render Button Edge
                Color edgeColorSelection = _MouseHover ? edgeHover : edgeColor;
                Pen pen = new Pen(_MouseDown ? edgeClick : edgeColorSelection) {Width = _MouseDown ? 0.8f : 0.5f};
                graphics.DrawPath(pen, button);

                //Render Overlay
                GraphicsPath overlay = RoundedRect(_ButtonBounds, 2, true);
                graphics.FillPath(new SolidBrush(Color.FromArgb(_MouseDown ? 0 : _MouseHover ? 40 : 60, 255, 255, 255)), overlay);

                //Render Button Text
                graphics.DrawString(_ButtonText, buttonFont, new SolidBrush(Color.White), _ButtonBounds, GH_TextRenderingConstants.CenterCenter);

            }
        }



        public override GH_ObjectResponse RespondToMouseDown(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                RectangleF rec = _ButtonBounds;
                if (rec.Contains(e.CanvasLocation))
                {
                    _MouseDown = true;
                    Owner.OnDisplayExpired(false);
                    return GH_ObjectResponse.Capture;
                }
            }
            return base.RespondToMouseDown(sender, e);
        }


        public override GH_ObjectResponse RespondToMouseUp(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                RectangleF rec = _ButtonBounds;
                if (rec.Contains(e.CanvasLocation))
                {
                    if (_MouseDown)
                    {
                        _MouseDown = false;
                        _MouseHover = false;
                        Owner.OnDisplayExpired(false);

                        _ButtonClickAction(); //Run Action

                        return GH_ObjectResponse.Release;
                    }
                }
            }
            return base.RespondToMouseUp(sender, e);
        }


        public override GH_ObjectResponse RespondToMouseMove(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            if (_ButtonBounds.Contains(e.CanvasLocation))
            {
                _MouseHover = true;
                Owner.OnDisplayExpired(false);
                sender.Cursor = System.Windows.Forms.Cursors.Hand;
                return GH_ObjectResponse.Capture;
            }

            if (_MouseHover)
            {
                _MouseHover = false;
                Owner.OnDisplayExpired(false);
                Grasshopper.Instances.CursorServer.ResetCursor(sender);
                return GH_ObjectResponse.Release;
            }

            return base.RespondToMouseMove(sender, e);
        }


        public static GraphicsPath RoundedRect(RectangleF bounds, int radius, bool overlay = false)
        {
            RectangleF b = new RectangleF(bounds.X, bounds.Y, bounds.Width, bounds.Height);
            int diameter = radius * 2;
            Size size = new Size(diameter, diameter);
            RectangleF arc = new RectangleF(b.Location, size);
            GraphicsPath path = new GraphicsPath();
            
            if (overlay)
                b.Height = diameter;

            if (radius == 0)
            {
                path.AddRectangle(b);
                return path;
            }

            // top left arc  
            path.AddArc(arc, 180, 90);

            // top right arc  
            arc.X = b.Right - diameter;
            path.AddArc(arc, 270, 90);

            if (!overlay)
            {
                // bottom right arc  
                arc.Y = b.Bottom - diameter;
                path.AddArc(arc, 0, 90);

                // bottom left arc 
                arc.X = b.Left;
                path.AddArc(arc, 90, 90);
            }
            else
            {
                path.AddLine(new PointF(b.X + b.Width, b.Y + b.Height), new PointF(b.X, b.Y + b.Height));
            }

            path.CloseFigure();
            return path;
        }
    }
}