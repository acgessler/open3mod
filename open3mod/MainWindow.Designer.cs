using System.Windows.Forms;
using OpenTK.Graphics;

namespace open3mod
{
    partial class MainWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            _ui.Dispose();
            _renderer.Dispose();

            if (disposing)
            {               
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.recentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.quitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.logViewerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.openToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.saveToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonFullView = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonTwoViews = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonFourViews = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonWireframe = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonShowTextures = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonShowShaded = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonShowFPS = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.helpToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeAllButThisToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.buttonTabClose = new System.Windows.Forms.Button();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.toolStripButtonShowBB = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonShowNormals = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonShowSkeleton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.glControl1 = new open3mod.RenderControl();
            this.menuStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.tabContextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Visible;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.toolStripMenuItem2,
            this.toolsToolStripMenuItem,
            this.toolStripMenuItem1});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1051, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.closeAllToolStripMenuItem,
            this.toolStripSeparator2,
            this.recentToolStripMenuItem,
            this.toolStripSeparator3,
            this.quitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(118, 22);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.OnFileMenuOpen);
            // 
            // closeAllToolStripMenuItem
            // 
            this.closeAllToolStripMenuItem.Name = "closeAllToolStripMenuItem";
            this.closeAllToolStripMenuItem.Size = new System.Drawing.Size(118, 22);
            this.closeAllToolStripMenuItem.Text = "Close all";
            this.closeAllToolStripMenuItem.Click += new System.EventHandler(this.OnFileMenuCloseAll);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(115, 6);
            // 
            // recentToolStripMenuItem
            // 
            this.recentToolStripMenuItem.Name = "recentToolStripMenuItem";
            this.recentToolStripMenuItem.Size = new System.Drawing.Size(118, 22);
            this.recentToolStripMenuItem.Text = "Recent";
            this.recentToolStripMenuItem.Click += new System.EventHandler(this.OnFileMenuRecent);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(115, 6);
            // 
            // quitToolStripMenuItem
            // 
            this.quitToolStripMenuItem.Name = "quitToolStripMenuItem";
            this.quitToolStripMenuItem.Size = new System.Drawing.Size(118, 22);
            this.quitToolStripMenuItem.Text = "Quit";
            this.quitToolStripMenuItem.Click += new System.EventHandler(this.OnFileMenuQuit);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem3});
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(44, 20);
            this.toolStripMenuItem2.Text = "View";
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.CheckOnClick = true;
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(129, 22);
            this.toolStripMenuItem3.Text = "Wireframe";
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.logViewerToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.toolsToolStripMenuItem.Text = "Tools";
            this.toolsToolStripMenuItem.Click += new System.EventHandler(this.ToolsToolStripMenuItemClick);
            // 
            // logViewerToolStripMenuItem
            // 
            this.logViewerToolStripMenuItem.Name = "logViewerToolStripMenuItem";
            this.logViewerToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
            this.logViewerToolStripMenuItem.Text = "Log Viewer";
            this.logViewerToolStripMenuItem.Click += new System.EventHandler(this.OnShowLogViewer);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(24, 20);
            this.toolStripMenuItem1.Text = "?";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.AboutToolStripMenuItemClick);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Location = new System.Drawing.Point(0, 712);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1051, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1,
            this.openToolStripButton,
            this.saveToolStripButton,
            this.toolStripSeparator4,
            this.toolStripButtonFullView,
            this.toolStripButtonTwoViews,
            this.toolStripButtonFourViews,
            this.toolStripSeparator,
            this.toolStripButtonWireframe,
            this.toolStripButtonShowTextures,
            this.toolStripButtonShowShaded,
            this.toolStripButtonShowFPS,
            this.toolStripSeparator1,
            this.toolStripButtonShowBB,
            this.toolStripButtonShowNormals,
            this.toolStripButtonShowSkeleton,
            this.toolStripSeparator5,
            this.helpToolStripButton});
            this.toolStrip1.Location = new System.Drawing.Point(0, 24);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1051, 25);
            this.toolStrip1.TabIndex = 2;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton1.Text = "toolStripButton1";
            // 
            // openToolStripButton
            // 
            this.openToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.openToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("openToolStripButton.Image")));
            this.openToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.openToolStripButton.Name = "openToolStripButton";
            this.openToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.openToolStripButton.Text = "&Open";
            // 
            // saveToolStripButton
            // 
            this.saveToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.saveToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("saveToolStripButton.Image")));
            this.saveToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.saveToolStripButton.Name = "saveToolStripButton";
            this.saveToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.saveToolStripButton.Text = "&Save";
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButtonFullView
            // 
            this.toolStripButtonFullView.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonFullView.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonFullView.Image")));
            this.toolStripButtonFullView.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonFullView.Name = "toolStripButtonFullView";
            this.toolStripButtonFullView.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonFullView.Text = "Full View";
            this.toolStripButtonFullView.Click += new System.EventHandler(this.ToggleFullView);
            // 
            // toolStripButtonTwoViews
            // 
            this.toolStripButtonTwoViews.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonTwoViews.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonTwoViews.Image")));
            this.toolStripButtonTwoViews.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonTwoViews.Name = "toolStripButtonTwoViews";
            this.toolStripButtonTwoViews.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonTwoViews.Text = "Two Views";
            this.toolStripButtonTwoViews.Click += new System.EventHandler(this.ToggleTwoViews);
            // 
            // toolStripButtonFourViews
            // 
            this.toolStripButtonFourViews.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonFourViews.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonFourViews.Image")));
            this.toolStripButtonFourViews.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonFourViews.Name = "toolStripButtonFourViews";
            this.toolStripButtonFourViews.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonFourViews.Text = "Four Views";
            this.toolStripButtonFourViews.Click += new System.EventHandler(this.ToggleFourViews);
            // 
            // toolStripSeparator
            // 
            this.toolStripSeparator.Name = "toolStripSeparator";
            this.toolStripSeparator.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButtonWireframe
            // 
            this.toolStripButtonWireframe.CheckOnClick = true;
            this.toolStripButtonWireframe.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonWireframe.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonWireframe.Image")));
            this.toolStripButtonWireframe.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonWireframe.Name = "toolStripButtonWireframe";
            this.toolStripButtonWireframe.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonWireframe.Text = "Enable Wireframe Mode";
            this.toolStripButtonWireframe.Click += new System.EventHandler(this.ToggleWireframe);
            // 
            // toolStripButtonShowTextures
            // 
            this.toolStripButtonShowTextures.CheckOnClick = true;
            this.toolStripButtonShowTextures.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonShowTextures.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonShowTextures.Image")));
            this.toolStripButtonShowTextures.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonShowTextures.Name = "toolStripButtonShowTextures";
            this.toolStripButtonShowTextures.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonShowTextures.Text = "Enable Textures";
            this.toolStripButtonShowTextures.Click += new System.EventHandler(this.ToggleTextures);
            // 
            // toolStripButtonShowShaded
            // 
            this.toolStripButtonShowShaded.CheckOnClick = true;
            this.toolStripButtonShowShaded.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonShowShaded.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonShowShaded.Image")));
            this.toolStripButtonShowShaded.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonShowShaded.Name = "toolStripButtonShowShaded";
            this.toolStripButtonShowShaded.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonShowShaded.Text = "Enable Shading";
            this.toolStripButtonShowShaded.Click += new System.EventHandler(this.ToggleShading);
            // 
            // toolStripButtonShowFPS
            // 
            this.toolStripButtonShowFPS.CheckOnClick = true;
            this.toolStripButtonShowFPS.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonShowFPS.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonShowFPS.Image")));
            this.toolStripButtonShowFPS.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonShowFPS.Name = "toolStripButtonShowFPS";
            this.toolStripButtonShowFPS.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonShowFPS.Text = "Show Frames per Second (FPS)";
            this.toolStripButtonShowFPS.Click += new System.EventHandler(this.ToggleFps);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // helpToolStripButton
            // 
            this.helpToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.helpToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("helpToolStripButton.Image")));
            this.helpToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.helpToolStripButton.Name = "helpToolStripButton";
            this.helpToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.helpToolStripButton.Text = "He&lp";
            // 
            // tabControl1
            // 
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 49);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1051, 663);
            this.tabControl1.TabIndex = 3;
            this.tabControl1.Selected += new System.Windows.Forms.TabControlEventHandler(this.OnTabSelected);
            this.tabControl1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.OnShowTabContextMenu);
            // 
            // tabContextMenuStrip
            // 
            this.tabContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.closeToolStripMenuItem,
            this.closeAllButThisToolStripMenuItem});
            this.tabContextMenuStrip.Name = "tabContextMenuStrip";
            this.tabContextMenuStrip.Size = new System.Drawing.Size(162, 48);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.closeToolStripMenuItem.Text = "Close";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.OnCloseTabFromContextMenu);
            // 
            // closeAllButThisToolStripMenuItem
            // 
            this.closeAllButThisToolStripMenuItem.Name = "closeAllButThisToolStripMenuItem";
            this.closeAllButThisToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.closeAllButThisToolStripMenuItem.Text = "Close all but this";
            this.closeAllButThisToolStripMenuItem.Click += new System.EventHandler(this.OnCloseAllTabsButThisFromContextMenu);
            // 
            // buttonTabClose
            // 
            this.buttonTabClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonTabClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonTabClose.Location = new System.Drawing.Point(1023, 38);
            this.buttonTabClose.Name = "buttonTabClose";
            this.buttonTabClose.Size = new System.Drawing.Size(22, 25);
            this.buttonTabClose.TabIndex = 4;
            this.buttonTabClose.Text = "X";
            this.buttonTabClose.UseVisualStyleBackColor = true;
            this.buttonTabClose.Click += new System.EventHandler(this.OnCloseTab);
            // 
            // openFileDialog
            // 
            this.openFileDialog.Multiselect = true;
            // 
            // toolStripButtonShowBB
            // 
            this.toolStripButtonShowBB.CheckOnClick = true;
            this.toolStripButtonShowBB.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonShowBB.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonShowBB.Image")));
            this.toolStripButtonShowBB.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonShowBB.Name = "toolStripButtonShowBB";
            this.toolStripButtonShowBB.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonShowBB.Text = "Show Bounding Boxes";
            this.toolStripButtonShowBB.Click += new System.EventHandler(this.ToggleShowBB);
            // 
            // toolStripButtonShowNormals
            // 
            this.toolStripButtonShowNormals.CheckOnClick = true;
            this.toolStripButtonShowNormals.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonShowNormals.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonShowNormals.Image")));
            this.toolStripButtonShowNormals.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonShowNormals.Name = "toolStripButtonShowNormals";
            this.toolStripButtonShowNormals.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonShowNormals.Text = "Show Normals";
            this.toolStripButtonShowNormals.Click += new System.EventHandler(this.ToggleShowNormals);
            // 
            // toolStripButtonShowSkeleton
            // 
            this.toolStripButtonShowSkeleton.CheckOnClick = true;
            this.toolStripButtonShowSkeleton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonShowSkeleton.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonShowSkeleton.Image")));
            this.toolStripButtonShowSkeleton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonShowSkeleton.Name = "toolStripButtonShowSkeleton";
            this.toolStripButtonShowSkeleton.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonShowSkeleton.Text = "Show Skeleton";
            this.toolStripButtonShowSkeleton.Click += new System.EventHandler(this.ToggleShowSkeleton);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(6, 25);
            // 
            // glControl1
            // 
            this.glControl1.AllowDrop = true;
            this.glControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.glControl1.BackColor = System.Drawing.Color.Black;
            this.glControl1.Location = new System.Drawing.Point(177, 70);
            this.glControl1.Name = "glControl1";
            this.glControl1.Size = new System.Drawing.Size(742, 626);
            this.glControl1.TabIndex = 0;
            this.glControl1.VSync = true;
            this.glControl1.Load += new System.EventHandler(this.OnGlLoad);
            this.glControl1.DragDrop += new System.Windows.Forms.DragEventHandler(this.OnDrag);
            this.glControl1.DragEnter += new System.Windows.Forms.DragEventHandler(this.OnDragEnter);
            this.glControl1.Paint += new System.Windows.Forms.PaintEventHandler(this.GlPaint);
            this.glControl1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnKeyDown);
            this.glControl1.KeyUp += new System.Windows.Forms.KeyEventHandler(this.OnKeyUp);
            this.glControl1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnMouseDown);
            this.glControl1.MouseEnter += new System.EventHandler(this.OnMouseEnter);
            this.glControl1.MouseLeave += new System.EventHandler(this.OnMouseLeave);
            this.glControl1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.OnMouseMove);
            this.glControl1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.OnMouseUp);
            this.glControl1.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.OnPreviewKeyDown);
            this.glControl1.Resize += new System.EventHandler(this.OnGlResize);
            // 
            // MainWindow
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(1051, 734);
            this.Controls.Add(this.buttonTabClose);
            this.Controls.Add(this.glControl1);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new System.Drawing.Size(600, 500);
            this.Name = "MainWindow";
            this.Text = "Open 3D Model Viewer ";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.OnCloseForm);
            this.Load += new System.EventHandler(this.Form1Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.tabContextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton openToolStripButton;
        private System.Windows.Forms.ToolStripButton saveToolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton helpToolStripButton;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem recentToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem quitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem3;
        private System.Windows.Forms.ToolStripButton toolStripButtonWireframe;
        private System.Windows.Forms.ToolStripButton toolStripButtonShowTextures;
        private System.Windows.Forms.ToolStripButton toolStripButtonShowShaded;
        private System.Windows.Forms.ToolStripButton toolStripButtonShowFPS;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripButton toolStripButtonFullView;
        private System.Windows.Forms.ToolStripButton toolStripButtonTwoViews;
        private System.Windows.Forms.ToolStripButton toolStripButtonFourViews;
        private System.Windows.Forms.ToolStripMenuItem logViewerToolStripMenuItem;
        private RenderControl glControl1;
        private TabControl tabControl1;
        private ContextMenuStrip tabContextMenuStrip;
        private ToolStripMenuItem closeToolStripMenuItem;
        private ToolStripMenuItem closeAllButThisToolStripMenuItem;
        private Button buttonTabClose;
        private OpenFileDialog openFileDialog;
        private ToolStripButton toolStripButtonShowBB;
        private ToolStripButton toolStripButtonShowNormals;
        private ToolStripButton toolStripButtonShowSkeleton;
        private ToolStripSeparator toolStripSeparator5;
    }
}

