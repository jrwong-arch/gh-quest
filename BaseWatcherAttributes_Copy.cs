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
    public class BaseWatcherAttributesCopy : GH_ComponentAttributes
    {

        RectangleF _TitleBounds = new RectangleF();

        RectangleF _LaunchButtonBounds = new RectangleF();
        Action _LaunchButtonClickAction;
        bool _LaunchMouseDown = false;
        bool _LaunchMouseHover = false;


        RectangleF _SelectionDropdownBounds = new RectangleF();
        RectangleF _SelectionDropdownTextBounds = new RectangleF();
        RectangleF _SelectionDropdownButtonBounds = new RectangleF();
        string _SelectionDisplayText;
        List<RectangleF> _SelectionDropdownItemBounds = new List<RectangleF>();
        List<string> _SelectionDropdownList = new List<string>{"TEST", "TEST", "TEST", "TEST", "TEST", "TEST", "TEST", "TEST", "TEST"};
        RectangleF _SelectionDropdownFullBounds = new RectangleF();
        Action _SelectionDropdownClickAction;
        bool _SelectionUnfolded = false;
        bool _SelectionMouseHover = false;
        RectangleF _Scrollbar;
        float _ScrollStartY;
        float _DragMouseStartY;
        float _DeltaY;
        int _MaxNoRows = 5;
        bool _Drag;


        public BaseWatcherAttributesCopy(IGH_Component component, Action launchAction) : base(component)
        {
            _LaunchButtonClickAction = launchAction;
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


            //Set Selection Drowdown
            bool removeScroll = true;
            float contentScroll = 0;

            if (_SelectionUnfolded)
            {
                removeScroll = false;

                _SelectionDropdownItemBounds.Clear();
                for (int i = 0; i < _SelectionDropdownList.Count; i++)
                {
                    _SelectionDropdownItemBounds.Add(new RectangleF(_LaunchButtonBounds.X, _LaunchButtonBounds.Y + (i + 1) * buttonHeight + edgeOffset, _LaunchButtonBounds.Width, _LaunchButtonBounds.Height));
                }
                _SelectionDropdownFullBounds = new RectangleF(_LaunchButtonBounds.X, _LaunchButtonBounds.Y + buttonHeight + edgeOffset, _LaunchButtonBounds.Width, Math.Min(_SelectionDropdownList.Count, _MaxNoRows) * _LaunchButtonBounds.Height);

                //update component size if dropdown is unfolded to be able to capture mouseclicks
                Bounds = new RectangleF(Bounds.X, Bounds.Y, Bounds.Width, Bounds.Height + _SelectionDropdownFullBounds.Height + edgeOffset);

                if (_SelectionDropdownList.Count > _MaxNoRows)
                {
                    // setup size of scroll bar
                    _Scrollbar.X = _SelectionDropdownFullBounds.X + _SelectionDropdownFullBounds.Width - 8; // locate from right-side of dropdown area
                    // compute height based on number of items in list, but with a minimum size of 2 rows
                    _Scrollbar.Height = (float)Math.Max(2 * buttonHeight, _SelectionDropdownFullBounds.Height * ((double)_MaxNoRows / ((double)_SelectionDropdownList.Count)));
                    _Scrollbar.Width = 8; // width of mouse-grab area (actual scroll bar drawn later)

                    // vertical position (.Y)
                    if (_DeltaY + _ScrollStartY >= 0) // handle if user drags above starting point
                    {
                        // dragging downwards:
                        if (_SelectionDropdownFullBounds.Height - _Scrollbar.Height >= _DeltaY + _ScrollStartY) // handles if user drags below bottom point
                        {
                            // update scroll bar position for normal scroll event within bounds
                            _Scrollbar.Y = _SelectionDropdownFullBounds.Y + _DeltaY + _ScrollStartY;
                        }
                        else
                        {
                            // scroll reached bottom
                            _ScrollStartY = _SelectionDropdownFullBounds.Height - _Scrollbar.Height;
                            _DeltaY = 0;
                        }
                    }
                    else
                    {
                        // scroll reached top
                        _ScrollStartY = 0;
                        _DeltaY = 0;
                    }

                    // calculate moved position of content
                    float scrollBarMovedPercentage = (_SelectionDropdownFullBounds.Y - _Scrollbar.Y) / (_SelectionDropdownFullBounds.Height - _Scrollbar.Height);
                    float scrollContentHeight = _SelectionDropdownList.Count * buttonHeight - _SelectionDropdownFullBounds.Height;
                    contentScroll = scrollBarMovedPercentage * scrollContentHeight;
                }
            }

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




                //Render Selection Dropdown
                graphics.FillRectangle(backgroundColor, _SelectionDropdownBounds);
                graphics.DrawRectangle(pen, _SelectionDropdownBounds.X, _SelectionDropdownBounds.Y, _SelectionDropdownBounds.Width, _SelectionDropdownBounds.Height);
                graphics.DrawString("TEST", buttonFont, new SolidBrush(Color.Black), _SelectionDropdownBounds, GH_TextRenderingConstants.NearCenter);

                if (_SelectionUnfolded)
                {
                    Pen penborder = new Pen(Brushes.Gray);
                    penborder.Width = 0.8f;
                    for (int i = 0; i < _SelectionDropdownItemBounds.Count; i++)
                    {
                        RectangleF listItem = _SelectionDropdownItemBounds[i];
                        if (listItem.Y < _SelectionDropdownFullBounds.Y)
                        {
                            if (listItem.Y + listItem.Height < _SelectionDropdownFullBounds.Y)
                            {
                                _SelectionDropdownItemBounds[i] = new RectangleF();
                                continue;
                            }
                            else
                            {
                                listItem.Height = listItem.Height - (_SelectionDropdownFullBounds.Y - listItem.Y);
                                listItem.Y = _SelectionDropdownFullBounds.Y;
                                _SelectionDropdownItemBounds[i] = listItem;
                            }
                        }
                        else if (listItem.Y + listItem.Height > _SelectionDropdownFullBounds.Y + _SelectionDropdownFullBounds.Height)
                        {
                            if (listItem.Y > _SelectionDropdownFullBounds.Y + _SelectionDropdownFullBounds.Height)
                            {
                                _SelectionDropdownItemBounds[i] = new RectangleF();
                                continue;
                            }
                            else
                            {
                                listItem.Height = _SelectionDropdownFullBounds.Y + _SelectionDropdownFullBounds.Height - listItem.Y;
                                _SelectionDropdownItemBounds[i] = listItem;
                            }
                        }

                        // background
                        graphics.FillRectangle(backgroundColor, _SelectionDropdownItemBounds[i]);
                        // border
                        graphics.DrawRectangle(penborder, _SelectionDropdownItemBounds[i].X, _SelectionDropdownItemBounds[i].Y, _SelectionDropdownItemBounds[i].Width, _SelectionDropdownItemBounds[i].Height);
                        // text
                        if (_SelectionDropdownItemBounds[i].Height > 2)
                            graphics.DrawString(_SelectionDropdownList[i], buttonFont, new SolidBrush(Color.Black), _SelectionDropdownItemBounds[i], GH_TextRenderingConstants.NearCenter);
                    }
                    // border
                    graphics.DrawRectangle(pen, _SelectionDropdownFullBounds.X, _SelectionDropdownFullBounds.Y, _SelectionDropdownFullBounds.Width, _SelectionDropdownFullBounds.Height);

                    // draw vertical scroll bar
                    Brush scrollbar = new SolidBrush(Color.FromArgb(_Drag ? 160 : 120, Color.Black));
                    Pen scrollPen = new Pen(scrollbar);
                    scrollPen.Width = _Scrollbar.Width - 2;
                    scrollPen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
                    scrollPen.EndCap = System.Drawing.Drawing2D.LineCap.Round;
                    graphics.DrawLine(scrollPen, _Scrollbar.X + 4, _Scrollbar.Y + 4, _Scrollbar.X + 4, _Scrollbar.Y + _Scrollbar.Height - 4);
                }
                else
                {

                }
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

                RectangleF selectionRectangle = _SelectionDropdownBounds;
                if (selectionRectangle.Contains(e.CanvasLocation))
                {
                    _SelectionUnfolded = !_SelectionUnfolded;
                    Owner.ExpireSolution(true);
                    return GH_ObjectResponse.Handled;
                }

                if(_SelectionUnfolded)
                {
                }
            }
            return base.RespondToMouseDown(sender, e);
        }


        public override GH_ObjectResponse RespondToMouseUp(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                RectangleF rec = _LaunchButtonBounds;
                if (rec.Contains(e.CanvasLocation))
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