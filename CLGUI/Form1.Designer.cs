namespace CLGUI {
    partial class Form1 {
        /// <summary>
        /// Variável de designer necessária.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpar os recursos que estão sendo usados.
        /// </summary>
        /// <param name="disposing">true se for necessário descartar os recursos gerenciados; caso contrário, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código gerado pelo Windows Form Designer

        /// <summary>
        /// Método necessário para suporte ao Designer - não modifique 
        /// o conteúdo deste método com o editor de código.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optimizeXMLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sP2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.recompressToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.keepFramePointsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dumpDDSToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.debugToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createRectanglesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.processToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dDSToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.generateStringPatchxmlToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.extractPackgetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.repackPACToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.keepIDAndLabelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // listBox1
            // 
            this.listBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(12, 12);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(380, 368);
            this.listBox1.TabIndex = 0;
            this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.Location = new System.Drawing.Point(12, 386);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(380, 20);
            this.textBox1.TabIndex = 1;
            this.textBox1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox1_KeyDown);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.optimizeXMLToolStripMenuItem,
            this.sP2ToolStripMenuItem,
            this.debugToolStripMenuItem,
            this.dDSToolStripMenuItem,
            this.generateStringPatchxmlToolStripMenuItem,
            this.extractPackgetToolStripMenuItem,
            this.repackPACToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(185, 224);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // optimizeXMLToolStripMenuItem
            // 
            this.optimizeXMLToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.keepIDAndLabelToolStripMenuItem});
            this.optimizeXMLToolStripMenuItem.Name = "optimizeXMLToolStripMenuItem";
            this.optimizeXMLToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.optimizeXMLToolStripMenuItem.Text = "Buid StringPatch.xml";
            this.optimizeXMLToolStripMenuItem.Click += new System.EventHandler(this.optimizeXMLToolStripMenuItem_Click);
            // 
            // sP2ToolStripMenuItem
            // 
            this.sP2ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem1,
            this.saveToolStripMenuItem1,
            this.dumpDDSToolStripMenuItem});
            this.sP2ToolStripMenuItem.Name = "sP2ToolStripMenuItem";
            this.sP2ToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.sP2ToolStripMenuItem.Text = "SP2";
            // 
            // openToolStripMenuItem1
            // 
            this.openToolStripMenuItem1.Name = "openToolStripMenuItem1";
            this.openToolStripMenuItem1.Size = new System.Drawing.Size(180, 22);
            this.openToolStripMenuItem1.Text = "Open";
            this.openToolStripMenuItem1.Click += new System.EventHandler(this.openToolStripMenuItem1_Click);
            // 
            // saveToolStripMenuItem1
            // 
            this.saveToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.recompressToolStripMenuItem,
            this.keepFramePointsToolStripMenuItem});
            this.saveToolStripMenuItem1.Name = "saveToolStripMenuItem1";
            this.saveToolStripMenuItem1.Size = new System.Drawing.Size(180, 22);
            this.saveToolStripMenuItem1.Text = "Save";
            this.saveToolStripMenuItem1.Click += new System.EventHandler(this.saveToolStripMenuItem1_Click);
            // 
            // recompressToolStripMenuItem
            // 
            this.recompressToolStripMenuItem.Checked = true;
            this.recompressToolStripMenuItem.CheckOnClick = true;
            this.recompressToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.recompressToolStripMenuItem.Name = "recompressToolStripMenuItem";
            this.recompressToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.recompressToolStripMenuItem.Text = "Recompress";
            // 
            // keepFramePointsToolStripMenuItem
            // 
            this.keepFramePointsToolStripMenuItem.CheckOnClick = true;
            this.keepFramePointsToolStripMenuItem.Name = "keepFramePointsToolStripMenuItem";
            this.keepFramePointsToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.keepFramePointsToolStripMenuItem.Text = "Keep Frame Points";
            // 
            // dumpDDSToolStripMenuItem
            // 
            this.dumpDDSToolStripMenuItem.Name = "dumpDDSToolStripMenuItem";
            this.dumpDDSToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.dumpDDSToolStripMenuItem.Text = "Dump DDS";
            this.dumpDDSToolStripMenuItem.Click += new System.EventHandler(this.dumpDDSToolStripMenuItem_Click);
            // 
            // debugToolStripMenuItem
            // 
            this.debugToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.createRectanglesToolStripMenuItem,
            this.processToolStripMenuItem});
            this.debugToolStripMenuItem.Name = "debugToolStripMenuItem";
            this.debugToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.debugToolStripMenuItem.Text = "Debug";
            // 
            // createRectanglesToolStripMenuItem
            // 
            this.createRectanglesToolStripMenuItem.Name = "createRectanglesToolStripMenuItem";
            this.createRectanglesToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.createRectanglesToolStripMenuItem.Text = "Create Rectangles";
            this.createRectanglesToolStripMenuItem.Click += new System.EventHandler(this.createRectanglesToolStripMenuItem_Click);
            // 
            // processToolStripMenuItem
            // 
            this.processToolStripMenuItem.Name = "processToolStripMenuItem";
            this.processToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.processToolStripMenuItem.Text = "Process";
            this.processToolStripMenuItem.Click += new System.EventHandler(this.processToolStripMenuItem_Click);
            // 
            // dDSToolStripMenuItem
            // 
            this.dDSToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem2,
            this.saveToolStripMenuItem2});
            this.dDSToolStripMenuItem.Name = "dDSToolStripMenuItem";
            this.dDSToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.dDSToolStripMenuItem.Text = "DDS";
            // 
            // openToolStripMenuItem2
            // 
            this.openToolStripMenuItem2.Name = "openToolStripMenuItem2";
            this.openToolStripMenuItem2.Size = new System.Drawing.Size(103, 22);
            this.openToolStripMenuItem2.Text = "Open";
            this.openToolStripMenuItem2.Click += new System.EventHandler(this.openToolStripMenuItem2_Click);
            // 
            // saveToolStripMenuItem2
            // 
            this.saveToolStripMenuItem2.Name = "saveToolStripMenuItem2";
            this.saveToolStripMenuItem2.Size = new System.Drawing.Size(103, 22);
            this.saveToolStripMenuItem2.Text = "Save";
            this.saveToolStripMenuItem2.Click += new System.EventHandler(this.saveToolStripMenuItem2_Click);
            // 
            // generateStringPatchxmlToolStripMenuItem
            // 
            this.generateStringPatchxmlToolStripMenuItem.Name = "generateStringPatchxmlToolStripMenuItem";
            this.generateStringPatchxmlToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.generateStringPatchxmlToolStripMenuItem.Text = "Dump Strings";
            this.generateStringPatchxmlToolStripMenuItem.Click += new System.EventHandler(this.generateStringPatchxmlToolStripMenuItem_Click);
            // 
            // extractPackgetToolStripMenuItem
            // 
            this.extractPackgetToolStripMenuItem.Name = "extractPackgetToolStripMenuItem";
            this.extractPackgetToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.extractPackgetToolStripMenuItem.Text = "Extract Packget";
            this.extractPackgetToolStripMenuItem.Click += new System.EventHandler(this.extractPackgetToolStripMenuItem_Click);
            // 
            // repackPACToolStripMenuItem
            // 
            this.repackPACToolStripMenuItem.Name = "repackPACToolStripMenuItem";
            this.repackPACToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.repackPACToolStripMenuItem.Text = "Repack PAC";
            this.repackPACToolStripMenuItem.Click += new System.EventHandler(this.repackPACToolStripMenuItem_Click);
            // 
            // keepIDAndLabelToolStripMenuItem
            // 
            this.keepIDAndLabelToolStripMenuItem.CheckOnClick = true;
            this.keepIDAndLabelToolStripMenuItem.Name = "keepIDAndLabelToolStripMenuItem";
            this.keepIDAndLabelToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.keepIDAndLabelToolStripMenuItem.Text = "Keep ID and Label";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(404, 414);
            this.ContextMenuStrip = this.contextMenuStrip1;
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.listBox1);
            this.Name = "Form1";
            this.Text = "CLGUI";
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem generateStringPatchxmlToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem optimizeXMLToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sP2ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem debugToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem createRectanglesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem dDSToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem processToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem recompressToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem extractPackgetToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem repackPACToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem keepFramePointsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dumpDDSToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem keepIDAndLabelToolStripMenuItem;
    }
}

