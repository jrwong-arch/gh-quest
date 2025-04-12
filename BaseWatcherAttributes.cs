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

        RectangleF _LaunchButtonBounds = new RectangleF();
        Action _LaunchButtonClickAction;
        bool _LaunchMouseDown = false;
        bool _LaunchMouseHover = false;


        RectangleF _SelectionDropdownBounds = new RectangleF();
        Action _SelectionDropdownClickAction;
        bool _SelectionDropdownMouseDown = false;
        bool _SelectionDropdownMouseHover = false;


        public BaseWatcherAttributes(IGH_Component component, Action launchAction, Action selectionAction) : base(component)
        {
            _LaunchButtonClickAction = launchAction;
            _SelectionDropdownClickAction = selectionAction;
        }



        protected override void Layout()
        {
            base.Layout();

            float componentWidth = 100;
            float componentHeight = 50;
            Bounds = new RectangleF(Bounds.X - componentWidth/2, Bounds.Y, componentWidth, componentHeight);

            float edgeOffset = 3.0f;
            float buttonHeight = 25.0f;
            _TitleBounds = new RectangleF(Bounds.X + edgeOffset, Bounds.Top + componentHeight/2 - buttonHeight/2, Bounds.Width - 2 * edgeOffset, buttonHeight);
            
            //Set Analyze Button
            _LaunchButtonBounds = new RectangleF(Bounds.X + edgeOffset, Bounds.Bottom + edgeOffset, Bounds.Width - 2 * edgeOffset, buttonHeight);
            Bounds = new RectangleF(Bounds.X, Bounds.Y, Bounds.Width, Bounds.Height + buttonHeight + (2 * edgeOffset));

            
            //Set Selection Dropdown
            _SelectionDropdownBounds = new RectangleF(Bounds.X + edgeOffset, Bounds.Bottom + edgeOffset, Bounds.Width - 2 * edgeOffset, buttonHeight);
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
                Brush backgroundColor = new SolidBrush(Color.LightGray);

                Color edgeColor = Color.Black;
                Color edgeHover = Color.Gray;
                Color edgeClick = Color.White;


                //Render Title Text
                graphics.DrawString("GH Quest", titleFont, new SolidBrush(Color.Black), _TitleBounds, GH_TextRenderingConstants.CenterCenter);



                //Render Button
                GraphicsPath button = RoundedRect(_LaunchButtonBounds, 2);
                Brush buttonColor = _LaunchMouseHover ? hoverColor : normalColor;
                graphics.FillPath(_LaunchMouseDown ? clickedColor : buttonColor, button);

                //Render Button Edge
                Color edgeColorSelection = _LaunchMouseHover ? edgeHover : edgeColor;
                Pen pen = new Pen(_LaunchMouseDown ? edgeClick : edgeColorSelection) {Width = _LaunchMouseDown ? 0.8f : 0.5f};
                graphics.DrawPath(pen, button);

                //Render Overlay
                GraphicsPath overlay = RoundedRect(_LaunchButtonBounds, 2, true);
                graphics.FillPath(new SolidBrush(Color.FromArgb(_LaunchMouseDown ? 0 : _LaunchMouseHover ? 40 : 60, 255, 255, 255)), overlay);

                //Render Button Text
                graphics.DrawString("Analyze", buttonFont, new SolidBrush(Color.White), _LaunchButtonBounds, GH_TextRenderingConstants.CenterCenter);



                //Render Button
                GraphicsPath dropdown = RoundedRect(_SelectionDropdownBounds, 2);
                Brush dropdownColor = _SelectionDropdownMouseDown ? hoverColor : normalColor;
                graphics.FillPath(_SelectionDropdownMouseDown ? clickedColor : dropdownColor, dropdown);

                //Render Button Edge
                Color dropdownEdgeColorSelection = _SelectionDropdownMouseHover ? edgeHover : edgeColor;
                Pen dropdownPen = new Pen(_SelectionDropdownMouseDown ? edgeClick : dropdownEdgeColorSelection) {Width = _SelectionDropdownMouseDown ? 0.8f : 0.5f};
                graphics.DrawPath(dropdownPen, dropdown);

                //Render Overlay
                GraphicsPath dropdownOverlay = RoundedRect(_SelectionDropdownBounds, 2, true);
                graphics.FillPath(new SolidBrush(Color.FromArgb(_SelectionDropdownMouseDown ? 0 : _SelectionDropdownMouseHover ? 40 : 60, 255, 255, 255)), dropdownOverlay);

                //Render Button Text
                graphics.DrawString("Select Lesson", buttonFont, new SolidBrush(Color.White), _SelectionDropdownBounds, GH_TextRenderingConstants.CenterCenter);
            }

        }



        public override GH_ObjectResponse RespondToMouseDown(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                RectangleF buttonRectangle = _LaunchButtonBounds;
                if (buttonRectangle.Contains(e.CanvasLocation))
                {
                    _LaunchMouseDown = true;
                    Owner.OnDisplayExpired(false);
                    return GH_ObjectResponse.Capture;
                }

                RectangleF dropdownRectangle = _SelectionDropdownBounds;
                if (dropdownRectangle.Contains(e.CanvasLocation))
                {
                    _SelectionDropdownMouseDown = true;
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
                RectangleF buttonRectangle = _LaunchButtonBounds;
                if (buttonRectangle.Contains(e.CanvasLocation))
                {
                    if (_LaunchMouseDown)
                    {
                        _LaunchMouseDown = false;
                        _LaunchMouseHover = false;
                        Owner.OnDisplayExpired(false);

                        _LaunchButtonClickAction(); //Run Action

                        return GH_ObjectResponse.Release;
                    }
                }

                RectangleF dropdownRectangle = _SelectionDropdownBounds;
                if(dropdownRectangle.Contains(e.CanvasLocation))
                {
                    if (_SelectionDropdownMouseDown)
                    {
                        _SelectionDropdownMouseDown = false;
                        _SelectionDropdownMouseHover = false;
                        Owner.OnDisplayExpired(false);

                        _SelectionDropdownClickAction(); //Run Action

                        return GH_ObjectResponse.Release;
                    }
                }
            }
            return base.RespondToMouseUp(sender, e);
        }


        public override GH_ObjectResponse RespondToMouseMove(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            if (_LaunchButtonBounds.Contains(e.CanvasLocation))
            {
                _LaunchMouseHover = true;
                Owner.OnDisplayExpired(false);
                sender.Cursor = System.Windows.Forms.Cursors.Hand;
                return GH_ObjectResponse.Capture;
            }

            if (_LaunchMouseHover)
            {
                _LaunchMouseHover = false;
                Owner.OnDisplayExpired(false);
                Grasshopper.Instances.CursorServer.ResetCursor(sender);
                return GH_ObjectResponse.Release;
            }

            if (_SelectionDropdownBounds.Contains(e.CanvasLocation))
            {
                _SelectionDropdownMouseHover = true;
                Owner.OnDisplayExpired(false);
                sender.Cursor = System.Windows.Forms.Cursors.Hand;
                return GH_ObjectResponse.Capture;
            }

            if (_SelectionDropdownMouseHover)
            {
                _SelectionDropdownMouseHover = false;
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