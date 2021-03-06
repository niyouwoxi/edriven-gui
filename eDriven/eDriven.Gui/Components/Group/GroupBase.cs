using eDriven.Core.Geom;
using eDriven.Gui.Containers;
using eDriven.Gui.Events;
using eDriven.Gui.Layout;
using UnityEngine;
using Event=eDriven.Core.Events.Event;

namespace eDriven.Gui.Components
{
    public class GroupBase : Component, IViewport
    {
        ///<summary>
        /// Constructor
        ///</summary>
        public GroupBase()
        {
            MouseEnabled = false;
        }

        //----------------------------------
        //  contentWidth
        //---------------------------------- 
        
        private float _contentWidth;

        ///<summary>
        /// Content width
        ///</summary>
        public float ContentWidth
        {
            get { return _contentWidth; }
        }

        private void SetContentWidth(float value) 
        {
            if (value == _contentWidth)
                return;
            float oldValue = _contentWidth;
            _contentWidth = value;
            InvalidateSize();
            InvalidateDisplayList();
            InvalidateTransform();
            DispatchPropertyChangeEvent("contentWidth", oldValue, value);        
        }

        //----------------------------------
        //  contentHeight
        //---------------------------------- 
        
        private float _contentHeight;

        public float ContentHeight
        {
            get { return _contentHeight; }
        }

        /**
         *  
         */
        private void SetContentHeight(float value) 
        {            
            if (value == _contentHeight)
                return;
            var oldValue = _contentHeight;
            _contentHeight = value;
            InvalidateSize();
            InvalidateDisplayList();
            InvalidateTransform();
            DispatchPropertyChangeEvent("contentHeight", oldValue, value);        
        }

        ///<summary>
        ///</summary>
        ///<param name="width"></param>
        ///<param name="height"></param>
        public void SetContentSize(float width, float height)
        {
            //Debug.Log("SetContentSize: " + width + ", " + height);
            if ((width == _contentWidth) && (height == _contentHeight))
               return;
            SetContentWidth(width);
            SetContentHeight(height);
        }

        #region Layout

        private LayoutBase _layout;
        private LayoutProperties _layoutProperties;
        private bool _layoutInvalidateSizeFlag;
        private bool _layoutInvalidateDisplayListFlag;

        /// <summary>
        /// The layout for this group
        /// </summary>
        public virtual LayoutBase Layout
        {
            get
            {
                return _layout;
            }
            set
            {
                /**
                 *  
                 * 
                 *  Three properties are delegated to the layout: clipAndEnableScrolling,
                 *  verticalScrollPosition, horizontalScrollPosition.
                 *  If the layout is reset, we copy the properties from the old
                 *  layout to the new one (we don't copy verticalScrollPosition
                 *  and horizontalScrollPosition from an old layout object to a new layout 
                 *  object because this information might not translate correctly).   
                 *  If the new layout is null, then we
                 *  temporarily store the delegated properties in _layoutProperties. 
                 */
                if (_layout == value)
                    return;
                
                if (null != _layout)
                {
                    _layout.Target = null;
                    _layout.RemoveEventListener(PropertyChangeEvent.PROPERTY_CHANGE, RedispatchLayoutEvent);

                    if (_clipAndEnableScrollingExplicitlySet)
                    {
                        // when the layout changes, we don't want to transfer over 
                        // horizontalScrollPosition and verticalScrollPosition
                        _layoutProperties = new LayoutProperties
                                                {
                                                    ClipAndEnableScrolling = _layout.ClipAndEnableScrolling
                                                };
                    }
                }

                _layout = value; 

                if (null != _layout)
                {
                    _layout.Target = this;
                    _layout.AddEventListener(PropertyChangeEvent.PROPERTY_CHANGE, RedispatchLayoutEvent);

                    if (null != _layoutProperties)
                    {
                        if (null != _layoutProperties.ClipAndEnableScrolling)
                            _layout.ClipAndEnableScrolling = (bool) _layoutProperties.ClipAndEnableScrolling;
                        
                        if (null != _layoutProperties.VerticalScrollPosition)
                            _layout.VerticalScrollPosition = (float) _layoutProperties.VerticalScrollPosition;
                        
                        if (null != _layoutProperties.HorizontalScrollPosition)
                            _layout.HorizontalScrollPosition = (float) _layoutProperties.HorizontalScrollPosition;
                        
                        _layoutProperties = null;
                    }
                }

                InvalidateSize();
                InvalidateDisplayList();
            }
        }

        private void RedispatchLayoutEvent(Event e)
        {
            PropertyChangeEvent pce = e as PropertyChangeEvent;
            if (null != pce)
            {
                switch (pce.Property)
                {
                    case "verticalScrollPosition":
                    case "horizontalScrollPosition":
                        DispatchEvent(e);
                        break;
                }
            }
        }

        #endregion

        public float HorizontalScrollPosition
        {
            get
            {
                if (null != _layout)
                {
                    return _layout.HorizontalScrollPosition;
                }
                if (null != _layoutProperties && 
                    null != _layoutProperties.HorizontalScrollPosition)
                {
                    return (float) _layoutProperties.HorizontalScrollPosition;
                }
                return 0;
            }
            set
            {
                if (null != _layout)
                {
                    _layout.HorizontalScrollPosition = value;
                }
                else if (null != _layoutProperties)
                {
                    _layoutProperties.HorizontalScrollPosition = value;
                }
                else
                {
                    _layoutProperties = new LayoutProperties
                                            {
                                                HorizontalScrollPosition = value
                                            };
                }
                //DispatchEvent(PropertyChangeEvent.CreateUpdateEvent(this, "horizontalScrollPosition", value, null));
                DispatchPropertyChangeEvent("horizontalScrollPosition", value, null);
                //if (null != _contentPane)
                //    _contentPane.InvalidateTransform(); // important: too early: the layout changes values in another cycle: doing it in CommitProperties()
            }
        }

        public float VerticalScrollPosition
        {
            get
            {
                if (null != _layout)
                {
                    return _layout.VerticalScrollPosition;
                }
                if (null != _layoutProperties &&
                    null != _layoutProperties.VerticalScrollPosition)
                {
                    return (float)_layoutProperties.VerticalScrollPosition;
                }
                return 0;
            }
            set
            {
                if (null != _layout)
                {
                    _layout.VerticalScrollPosition = value;
                }
                else if (null != _layoutProperties)
                {
                    _layoutProperties.VerticalScrollPosition = value;
                }
                else
                {
                    _layoutProperties = new LayoutProperties
                    {
                        VerticalScrollPosition = value
                    };
                }
                //DispatchEvent(PropertyChangeEvent.CreateUpdateEvent(this, "verticalScrollPosition", value, null));
                DispatchPropertyChangeEvent("verticalScrollPosition", value, null);
                //if (null != _contentPane)
                //    _contentPane.InvalidateTransform();  // important: too early: the layout changes values in another cycle: doing it in CommitProperties()
            }
        }

        //----------------------------------
        //  clipAndEnableScrolling
        //----------------------------------

        private bool _clipAndEnableScrollingExplicitlySet;

        ///<summary>
        /// Clip and enable scrolling
        ///</summary>
        public bool ClipAndEnableScrolling
        {
            get
            {
                if (null != _layout)
                {
                    return _layout.ClipAndEnableScrolling;
                }
                if (null != _layoutProperties && 
                    null != _layoutProperties.ClipAndEnableScrolling)
                {
                    return (bool) _layoutProperties.ClipAndEnableScrolling;
                }
                return false;
            }
            set { 
                _clipAndEnableScrollingExplicitlySet = true;
                if (null != _layout)
                {
                    _layout.ClipAndEnableScrolling = value;
                }
                else if (null != _layoutProperties)
                {
                    _layoutProperties.ClipAndEnableScrolling = value;
                }
                else
                {
                    _layoutProperties = new LayoutProperties
                                            {
                                                ClipAndEnableScrolling = value
                                            };
                }

                // clipAndEnableScrolling affects measured minimum size
                InvalidateSize();
                InvalidateTransform();
            }
        }

        ///<summary>
        ///</summary>
        ///<param name="navigationUnit"></param>
        ///<returns></returns>
        public float GetHorizontalScrollPositionDelta(NavigationUnit navigationUnit)
        {
            return (null != _layout) ? _layout.GetHorizontalScrollPositionDelta(navigationUnit) : 0;   
        }

        ///<summary>
        ///</summary>
        ///<param name="navigationUnit"></param>
        ///<returns></returns>
        public float GetVerticalScrollPositionDelta(NavigationUnit navigationUnit)
        {
            return (null != _layout) ? _layout.GetVerticalScrollPositionDelta(navigationUnit) : 0;
        }

        /**
         *  
         *  Storage for the autoLayout property.
         */
        private bool _autoLayout = true; // NOTE: True by default!!!

        ///<summary>
        /// If true, measurement and layout are done when the position or size of a child is changed.
        /// If false, measurement and layout are done only once, when children are added to or removed from the container.
        ///</summary>
        public virtual bool AutoLayout
        {
            get
            {
                return _autoLayout;
            }
            set
            {
                if (value == _autoLayout)
                    return;

                _autoLayout = value;

                // If layout is being turned back on, trigger a layout to occur now.
                if (_autoLayout)
                {
                    InvalidateSize();
                    InvalidateDisplayList();
                    InvalidateParentSizeAndDisplayList();
                }
            }   
        }

        private bool _scrollRectSet;

        /// <summary>
        /// TODO: This is buggy. Handle ip properly.
        /// </summary>
        public Rectangle ScrollRect
        {
            get // todo: implement content pane!
            {
                return new Rectangle(0, 0, Width, Height);
            }
            set // todo: implement content pane!
            {
                //if (Id == "contentGroup")
                //    Debug.Log("Setting ScrollRect to " + value);

                _needToPositionContentPaneIfExists = true;
                InvalidateProperties();

                if (!_scrollRectSet && null == value)
                    return;
                _scrollRectSet = true;
                //super.scrollRect = value;

                //CreateContentPane();
                if (null == _contentPane)
                {
                    _needToCreateContentPane = true;
                    InvalidateProperties();
                }

                //if (null != value)
                //{
                //    SetActualSize(value.Width, value.Height);
                //    //Width = value.Width; // ? // NONO! NEVER set width and height of the container -> the error with HGroup 20131018
                //    //Height = value.Height; // ?    
                //}
                //else
                //{
                //    // TODO
                //}
            }
        }

        public override void InvalidateSize()
        {
            //if (Id == "hbox")
            //    Debug.Log("InvalidateSize: " + this);
            base.InvalidateSize();
            _layoutInvalidateSizeFlag = true;
        }

        public override void InvalidateDisplayList()
        {
            base.InvalidateDisplayList();
            _layoutInvalidateDisplayListFlag = true;
        }

        public void QInvalidateSize()
        {
            base.InvalidateSize();
        }

        public void QInvalidateDisplayList()
        {
            base.InvalidateDisplayList();
        }

        internal override void ChildXYChanged()
        {
            if (AutoLayout)
            {
                InvalidateSize();
                InvalidateDisplayList();
            }
        }

        protected override void CreateChildren()
        {
            base.CreateChildren();

            if (null == _layout)
                Layout = new AbsoluteLayout(); // Important: via setter
        }

        protected override void Measure()
        {
            //if (Id == "hbox")
            //    Debug.Log("Measure: " + this);

            if (null != _layout && _layoutInvalidateSizeFlag)
            {
                float oldMeasuredWidth = MeasuredWidth;
                float oldMeasuredHeight = MeasuredHeight;
                
                base.Measure();
            
                _layoutInvalidateSizeFlag = false;
                _layout.Measure();

                // Special case: If the group clips content, or resizeMode is "scale"
                // then measured minimum size is zero
                if (ClipAndEnableScrolling/* || resizeMode == ResizeMode.SCALE*/)
                {
                    MeasuredMinWidth = 0;
                    MeasuredMinHeight = 0;
                }

                //if (MeasuredWidth != oldMeasuredWidth || MeasuredHeight != oldMeasuredHeight) // commented 20131024 - not ResizeMode.SCALE
                //    InvalidateDisplayList();

                //if (Id == "hbox")
                //    Debug.Log("  --> MeasuredWidth: " + MeasuredWidth);
            }
        }

        protected override void UpdateDisplayList(float width, float height)
        {
            //if (Id == "hbox")
            //    Debug.Log("UpdateDisplayList: " + width + "; " + height);

            //if (Id == "contentGroup")
            //    Debug.Log("GroupBase.UpdateDisplayList: " + width + ", " + height);

            base.UpdateDisplayList(width, height);

            if (_layoutInvalidateDisplayListFlag)
            {
                _layoutInvalidateDisplayListFlag = false;
                if (_autoLayout && null != _layout)
                    _layout.UpdateDisplayList(width, height);

                if (null != _layout)
                    _layout.UpdateScrollRect(width, height);
            }

            if (_drawingListInvalidated)
            {
                //Debug.Log("Calling DepthUtil.UpdateDrawingList on " + this);
                _drawingListInvalidated = false;
                //DepthUtil.UpdateDrawingList(this);
                DepthUtil.UpdateDrawingList(_contentPane ?? this);
            }
        }

        #region Content pane

        internal DisplayObjectContainer _contentPane;

        /// <summary>
        /// Note: The layout cannot run the content pane creation directly because by that time the children 
        /// have not yet been created, so no children are being moved to content pane
        /// </summary>
        private bool _needToCreateContentPane;

        /// <summary>
        /// Note: The layout cannot position the content pane directly because by that time it
        /// could have not yet been created, so we need to invalidate a property flag
        /// </summary>
        private bool _needToPositionContentPaneIfExists;

        /// <summary>
        /// Commit properties
        /// </summary>
        protected override void CommitProperties()
        {
            base.CommitProperties();

            if (_needToCreateContentPane)
            {
                _needToCreateContentPane = false;
                CreateContentPane();
            }

            if (_needToPositionContentPaneIfExists)
            {
                _needToPositionContentPaneIfExists = false;
                if (null != _contentPane) {
                    //Debug.Log("RenderingRect: " + RenderingRect);
                    _contentPane.X = ClipAndEnableScrolling ? -HorizontalScrollPosition : 0; //RenderingRect.x;
                    _contentPane.Y = ClipAndEnableScrolling ? -VerticalScrollPosition : 0; //RenderingRect.y;
                    // we have to apply, because setting X and Y to 0 on a DisplayObjectContainer doesn't apply automatically
                    // (so the content pane stays in the top left corner of the screen)
                    _contentPane.InvalidateTransform(); // important: no scrolling without applying
                }
            }
        }

        private bool _creatingContentPane;
        
        /// <summary>
        /// A flag that has to be switched ON if moving children from one parent to another by the system
        /// (this is usually done when moving to or from content pane)
        /// After moving children it has to be set OFF
        /// </summary>
// ReSharper disable MemberCanBePrivate.Global
        internal bool CreatingContentPane
// ReSharper restore MemberCanBePrivate.Global
        {
            set { _creatingContentPane = value; }
        }

        private void CreateContentPane()
        {
            if (null != _contentPane)
                return;

            //Debug.Log("Creating content pane");

            CreatingContentPane = true;

            var n = NumberOfChildren; // snapshot now
            //Debug.Log("CreateContentPane. Number of children: " + n);

            System.Collections.Generic.List<DisplayListMember> childrenToMove = new System.Collections.Generic.List<DisplayListMember>();
            for (int i = 0; i < n; i++)
            {
                childrenToMove.Add(base.GetChildAt(i));
                //Debug.Log("Will move: " + base.GetChildAt(i));
            }

            /**
             * Content pane is a simple display object
             * NOTE: we have to use temp variable here
             * The reason is this line below: newPane.AddChild(child); 
             * If the _contentPane is not null, this changed the flow: 
             * we are expecting that AddChild() indirectly calls the RemoveChild() on parent (meaning: this container)
             * However, if _contentPane alsready set, it will try to remove the child from the pane itself!
             * */
#if DEBUG
            var newPane = new DisplayObjectContainer
            {
                Id = "content_pane", // for debugging purposes
                X = 0,
                Y = 0,
                AutoUpdateDrawingList = false,
                Visible = true
            };
#endif
#if !DEBUG
            var newPane = new DisplayObjectContainer
            {
                X = 0,
                Y = 0,
                AutoUpdateDrawingList = false,
                Visible = true
            };
#endif
            /**
             * Add content pane as a last child 
             * (cannot use AddChild(_contentPane) here, because it takes the number of 
             * children internally depending of the pane existance)
             * Important:
             * Also cannot use AddChildAt, since it would then try to add content pane to the content pane itself 
             * */
            base.AddingChild(newPane);
            QAddChildAt(newPane, n);
            base.ChildAdded(newPane);

            //var mover = new ChildMover(this, _contentPane, numberOfChildren);
            //mover.Move();

            foreach (DisplayListMember child in childrenToMove)
            {
                // set the container as a parent
                var cmp = child as Component;

                //RemoveChild(child); // TODO: remove
                newPane.AddChild(child); // AddChild interno zove RemoveChild na OVOM kontejneru. Zbog toga je potrebno da je _contentPane == null
                if (null != cmp)
                    cmp.ParentChanged(newPane);

                //Debug.Log("    ... done");
            }

            _contentPane = newPane;

            //Debug.Log("NumberOfChildren: " + NumberOfChildren);
            //Debug.Log("_contentPane.NumberOfChildren: " + _contentPane.NumberOfChildren);

            DepthUtil.UpdateDrawingList(this); // important! Cannot call the InvalidateDrawingList here because it will never update the display list of this
            
            //DepthUtil.UpdateDrawingList(_contentPane); // auto update is turned off on the content pane
            InvalidateDrawingList(); // same as DepthUtil.UpdateDrawingList(_contentPane) but delayed (so perhaps better for performance)

            CreatingContentPane = false;

            _contentPane.Visible = true;
        }

        #endregion

        #region Implementation of IChildList

        /// <summary>
        /// The child components of the container
        /// </summary>
        public override System.Collections.Generic.List<DisplayListMember> Children
        {
            get { return null != _contentPane ? _contentPane.QChildren : QChildren; }
        }

        /// <summary>
        /// Number of elements
        /// </summary>
        public override int NumberOfChildren
        {
            get { return null != _contentPane ? _contentPane.QNumberOfChildren : QNumberOfChildren; }
        }

        /// <summary>
        /// Checks if this is a Owner of a component
        /// </summary>
        /// <param name="child"></param>
        /// <returns></returns>
        public override bool HasChild(DisplayListMember child)
        {
            return null != _contentPane ? _contentPane.QHasChild(child) : QHasChild(child);
        }

        public override bool Contains(DisplayListMember child)
        {
            return null != _contentPane ? _contentPane.QContains(child) : QContains(child);
        }

        public override bool Contains(DisplayListMember child, bool exclusive)
        {
            return null != _contentPane ? _contentPane.QContains(child, exclusive) : QContains(child, exclusive);
        }

        #region Add / remove child overrides

        /**
         * Važno:
         * Ove overrideove sam ja dodao 20131105
         * Razlog je bio problem oko dinamičkog dodavanja itema u List kontrolu
         * Naime, iako su se liste inicijalno normalno inicijalizirale, prilikom dodavanja novog childa u kolekciju (dinamički)
         * događao se Exception u DataGroup metodi AddItemRendererToDisplayList ("ArgumentOutOfRangeException: Argument is out of range.")
         * Greška je bila ta da je linija base.AddChildAt(child, childIndex) naravno zvala base.AddChildAt od klase Component
         * a ta implementacija interno zove QAddChildAt (tj. ne radi razliku između toga da li postoji contentPane ili ne)
         * Tako da je child uvijek bio dodavan direktno na GroupBase, a ne na contentPane ukoliko je postojao
         * (GroupBase "laže" vanjskom svijetu i skriva činjenicu da postoji contentPane)
         * Zbog toga je bilo potrebno overridati ove 4 metode i ispraviti tu funkcionalnost
         * */

        public override DisplayListMember AddChild(DisplayListMember child)
        {
            DisplayObjectContainer formerParent = child.Parent;
            if (null != formerParent)
                formerParent.RemoveChild(child);

            // Do anything that needs to be done before the child is added.
            // When adding a child to Component, this will set the child's
            // virtual parent, its nestLevel, its document, etc.
            // When adding a child to a Container, the override will also
            // invalidate the container, adjust its content/chrome partitions,
            // etc.
            //base.AddingChild(child);

            if (null != _contentPane)
                ContentPaneAddingChild(child);
            else
                base.AddingChild(child);

            // Call a low-level player method in DisplayObjectContainer which
            // actually attaches the child to this component.
            // The player dispatches an "added" event from the child just after
            // it is attached, so all "added" handlers execute during this call.
            // Component registers an addedHandler() in its constructor,
            // which makes it runs before any other "added" handlers except
            // capture-phase ones; it sets up the child's styles.
            var retVal = null != _contentPane ? _contentPane.QAddChild(child) : QAddChild(child);

            // Do anything that needs to be done after the child is added
            // and after all "added" handlers have executed.
            // This is where
            /*base.*/ChildAdded(child);

            InvalidateDrawingList();
            //if (null != _contentPane)
            //    DepthUtil.UpdateDrawingList(_contentPane); // because AutoUpdateDrawingList is turned of on contentPane

            return retVal;
        }

        public override DisplayListMember AddChildAt(DisplayListMember child, int index)
        {
            if (null != _contentPane)
                ContentPaneAddingChild(child);
            else
                base.AddingChild(child);

            var retVal = null != _contentPane ? _contentPane.QAddChildAt(child, index) : QAddChildAt(child, index);
            
            /*base.*/ChildAdded(child);

            InvalidateDrawingList();
            //if (null != _contentPane)
            //    DepthUtil.UpdateDrawingList(_contentPane); // because AutoUpdateDrawingList is turned of on contentPane

            return retVal;
        }

        /// <summary>
        /// Helper
        /// </summary>
        /// <param name="child"></param>
        private void ContentPaneAddingChild(DisplayListMember child)
        {
            //Debug.Log("ContentPaneAddingChild: " + child);
            var component = child as Component;
            if (component != null)
                component.ParentChanged(_contentPane); // fix the parent (currently it is "this")

            var client = child as IInvalidationManagerClient;
            if (client != null)
                client.NestLevel = /*NestLevel == -1 ? -1 : */NestLevel + 1;

            var styleClient = child as IStyleClient;
            if (styleClient != null)
                styleClient.RegenerateStyleCache(true);

            var simpleStyleClient = child as ISimpleStyleClient;
            if (simpleStyleClient != null)
                simpleStyleClient.StyleChanged(null);

            if (styleClient != null)
                styleClient.NotifyStyleChangeInChildren(null, null, true);

            // Inform the component that it's style properties
            // have been fully initialized. Most components won't care,
            // but some need to react to even this early change.
            if (component != null)
                component.StylesInitialized();
        }

        override public DisplayListMember RemoveChild(DisplayListMember child)
        {
            RemovingChild(child);

            var retVal = null != _contentPane ? _contentPane.QRemoveChild(child) : QRemoveChild(child);

            ChildRemoved(child);

            InvalidateDrawingList();

            return retVal;
        }

        override public DisplayListMember RemoveChildAt(int index) // TODO: Do delayed version
        {
            //DisplayListMember child = QGetChildAt(index);
            //return QRemoveChild(child);

            DisplayListMember child = GetChildAt(index);

            RemovingChild(child);

            var retVal = null != _contentPane ? _contentPane.QRemoveChild(child) : QRemoveChild(child);

            ChildRemoved(child);

            InvalidateDrawingList();

            return retVal;
        }

        /**
         * Note. we have to override this method because of the content pane
         * */
        public override void RemoveAllChildren()
        {
            for (int i = Children.Count - 1; i >= 0; i--)
            {
                RemoveChildAt(i);
            }
        }

        #endregion

        //public override DisplayListMember RemoveChild(DisplayListMember child)
        //{
        //    return null != _contentPane ? _contentPane.QRemoveChild(child) : QRemoveChild(child);
        //}

        //public override DisplayListMember RemoveChildAt(int index)
        //{
        //    return null != _contentPane ? _contentPane.QRemoveChildAt(index) : QRemoveChildAt(index);
        //}

        //public override void RemoveAllChildren()
        //{
        //    if (null != _contentPane)
        //        _contentPane.QRemoveAllChildren();
        //    else
        //        QRemoveAllChildren();
        //}

        ///<summary>
        /// Swaps two children
        ///</summary>
        ///<param name="firstElement">First child</param>
        ///<param name="secondElement">Second child</param>
        public override void SwapChildren(DisplayListMember firstElement, DisplayListMember secondElement)
        {
            //Debug.Log("SwapChildren");
            if (null != _contentPane)
                _contentPane.QSwapChildren(firstElement, secondElement);
            else
                QSwapChildren(firstElement, secondElement);
        }

        /// <summary>
        /// Gets child at specified position
        /// </summary>
        /// <param name="index"></param>
        /// <returns>Child index</returns>
        public override DisplayListMember GetChildAt(int index)
        {
            //Debug.Log("GetChildAt: " + index + ", _contentPane:" + _contentPane);
            return null != _contentPane ? _contentPane.QGetChildAt(index) : QGetChildAt(index);
        }

        /// <summary>
        /// Gets child index
        /// </summary>
        /// <param name="child">A child</param>
        /// <returns>The position</returns>
        public override int GetChildIndex(DisplayListMember child)
        {
            //Debug.Log("GetChildIndex: " + child);
            return null != _contentPane ? _contentPane.QGetChildIndex(child) : QGetChildIndex(child);
        }

        //public override void SetChildIndex(DisplayListMember child, int index)
        //{
        //    //Debug.Log("GetChildIndex: " + child);
        //    if (null != _contentPane)
        //        _contentPane.QSetChildIndex(child, index);
        //    else
        //        QSetChildIndex(child, index);
        //}

        #endregion

        #region Render

        /**
         * We are having the following problem here:
         * I don't wont to mess with rendering, because it introduces a lot of other problems:
         * - children coordinates never change so children are "natively" clickable
         * - in some future implementation the children of the scrollable container would be parented to the separate game object
         * - and also, it's faster to change a coordinates on the single object, then on multiple ones
         * So, I'd really like to have a "content pane" logic here (like in the Container class)
         * The problem is that there is already a bunch of layout variables here (layout is set indirectly, delayed if no content pane instantiated yet)
         * So we must be very careful when introducing out layout variables
         * We will definitelly need to remain the Q-methods, keeping the true nature of parent-child relationship
         * Hopefully, there won't be any inconveniences in the relation to skinning and style propagation (don't see any now)
         * */
        protected override void PreRender()
        {
            if (ClipAndEnableScrolling)
            {
                //GUI.BeginGroup(new Rect(200, 200, Width, Height));
                //Debug.Log("Clippint: " + RenderingRect);

                /**
                 * 1. Outer
                 * */
                GUI.BeginGroup(RenderingRect);

                //var inner = new Rect(RenderingRect) {x = -HorizontalScrollPosition, y = -VerticalScrollPosition};

                ///**
                // * 2. Inner
                // * */
                //GUI.BeginGroup(inner);
            }

            base.PreRender();
        }

        protected override void PostRender()
        {
            base.PostRender();

            if (ClipAndEnableScrolling)
            {
                /**
                 * 1. Inner
                 * */
                //GUI.EndGroup();

                /**
                 * 2. Outer
                 * */
                GUI.EndGroup();
            }
        }

        #endregion
        
        public virtual int NumberOfContentChildren
        {
            get
            {
                return -1;
            }
        }
        
        public virtual DisplayListMember GetContentChildAt(int index)
        {
            return null;
        }
        
        public virtual IVisualElement GetVirtualElementAt(int index, float eltWidth, float eltHeight)
        {
            return GetContentChildAt(index);            
        }
        
        public virtual int GetContentChildIndex(DisplayListMember child)
        {
            return -1;
        }

        #region Invalidate/update display list overrides

        private bool _drawingListInvalidated;
        internal override void InvalidateDrawingList()
        {
            _drawingListInvalidated = true;
            InvalidateDisplayList();
            //InvalidateProperties();
        }

        #endregion

        #region Overriding ChildAdded / RemovingChild because of ViewStack

        protected override void ChildAdded(DisplayListMember child)
        {
            //Debug.Log("GroupBase - ChildAdded: " + child);
            if (HasEventListener("childrenChanged"))
                DispatchEvent(new Event("childrenChanged"));

            if (HasEventListener(ChildExistenceChangedEvent.CHILD_ADD))
            {
                var cece = new ChildExistenceChangedEvent(ChildExistenceChangedEvent.CHILD_ADD) {RelatedObject = child};
                DispatchEvent(cece);
            }
            
            /*if (child is ModalOverlay)
                Debug.Log("Added: " + child);*/

            if (child.HasEventListener(FrameworkEvent.ADD))
                child.DispatchEvent(new FrameworkEvent(FrameworkEvent.ADD));

 	        base.ChildAdded(child);
        }

        protected override void RemovingChild(DisplayListMember child)
        {
            base.RemovingChild(child);

            if (child.HasEventListener(FrameworkEvent.REMOVE))
                child.DispatchEvent(new FrameworkEvent(FrameworkEvent.REMOVE));

            if (HasEventListener(ChildExistenceChangedEvent.CHILD_REMOVE))
            {
                var cece = new ChildExistenceChangedEvent(ChildExistenceChangedEvent.CHILD_REMOVE) {RelatedObject = child};
                DispatchEvent(cece);
            }
        }

        #endregion

    }
}