using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Timer = System.Threading.Timer;

namespace WpfApp1
{
    /// <summary>
    /// グラフィックデバイスコントロール
    /// </summary>
    public partial class GraphicsDeviceControl : Control
    {
        /// <summary>
        /// Direct3D デバイス
        /// </summary>
        private Device device = null;

        /// <summary>
        /// 頂点データ
        /// </summary>
        private CustomVertex.PositionColored[] vertices = new CustomVertex.PositionColored[3];

        private Timer _timer;


        /// <summary>
        /// 頂点データ
        /// </summary>
        public CustomVertex.PositionColored[] Vertices
        {
            get { return this.vertices; }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public GraphicsDeviceControl()
        {
            InitializeComponent();
            _timer = new Timer(state => Draw(), null, TimeSpan.Zero, TimeSpan.FromMilliseconds(1000 / 30));
        }

        /// <summary>
        /// コントロールが作成されるとき
        /// </summary>
        protected override void OnCreateControl()
        {
            if (this.DesignMode == false)
            {
                try
                {
                    // デバイス作成
                    PresentParameters pp = new PresentParameters();
                    pp.Windowed = true;
                    pp.SwapEffect = SwapEffect.Discard;
                    pp.BackBufferWidth = 300;
                    pp.BackBufferHeight = 300;
                    pp.EnableAutoDepthStencil = true;
                    pp.AutoDepthStencilFormat = DepthFormat.D16;
                    this.device = new Device(0, DeviceType.Hardware, this.Handle,
                        CreateFlags.SoftwareVertexProcessing, pp);

                    // 頂点データの設定
                    this.vertices[0] = new CustomVertex.PositionColored(
                        0.0f, -2.0f + (float)Math.Sqrt(3) * 3.0f, 0.0f,
                        Color.FromArgb(255, 0, 0).ToArgb());
                    this.vertices[1] = new CustomVertex.PositionColored(
                        3.0f, -2.0f, 0.0f,
                        Color.FromArgb(0, 255, 0).ToArgb());
                    this.vertices[2] = new CustomVertex.PositionColored(
                        -3.0f, -2.0f, 0.0f,
                        Color.FromArgb(0, 0, 255).ToArgb());

                    // ビュー変換行列を設定
                    this.device.Transform.View = Matrix.LookAtRH(
                        new Vector3(0.0f, 0.0f, -10.0f),
                        new Vector3(0.0f, 0.0f, 0.0f),
                        new Vector3(0.0f, 1.0f, 0.0f));

                    // 射影変換を設定
                    this.device.Transform.Projection = Matrix.PerspectiveFovRH(
                        Geometry.DegreeToRadian(45.0f), 1.0f, 1.0f, 100.0f);

                    // レンダリングステート設定
                    this.device.RenderState.Lighting = false;
                    this.device.RenderState.CullMode = Cull.None;
                    this.device.RenderState.AlphaBlendEnable = true;
                    this.device.RenderState.SourceBlend = Blend.SourceAlpha;
                    this.device.RenderState.DestinationBlend = Blend.InvSourceAlpha;
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.ToString());
                }
            }

            base.OnCreateControl();
        }

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            if (disposing)
            {
                if (this.device != null)
                {
                    this.device.Dispose();
                }

                if (_timer != null)
                {
                    _timer.Change(-1, int.MaxValue);
                    _timer.Dispose();
                }
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// 描画イベント
        /// </summary>
        /// <param name="pe"></param>
        protected override void OnPaint(PaintEventArgs pe)
        {
            this.Draw();
            base.OnPaint(pe);
        }

        /// <summary>
        /// 描画
        /// </summary>
        private void Draw()
        {
            if (this.device == null)
            {
                return;
            }

            this.device.BeginScene();
            this.device.Clear(ClearFlags.ZBuffer | ClearFlags.Target, Color.DarkBlue, 1.0f, 0);

            // 三角形描画
            this.device.Transform.World = Matrix.RotationY((float)Environment.TickCount / 1000.0f);
            this.device.VertexFormat = CustomVertex.PositionColored.Format;
            this.device.DrawUserPrimitives(PrimitiveType.TriangleList, 1, this.vertices);

            this.device.EndScene();
            this.device.Present();
        }
    }
}