using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.DirectX;
using Microsoft.DirectX.DirectInput;
using Key = Microsoft.DirectX.DirectInput.Key;

namespace WpfApp1
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        private System.Windows.Forms.Timer _timer;
        private Device _keyboardDevice;

        public MainWindow()
        {
            InitializeComponent();
            // キーボードデバイスの作成
            IntPtr hwd = new WindowInteropHelper(this).Handle;
            _keyboardDevice = new Device(SystemGuid.Keyboard);

            // 協調レベルの設定
            _keyboardDevice.SetCooperativeLevel(hwd,
                CooperativeLevelFlags.NonExclusive | CooperativeLevelFlags.Background);
            // キャプチャするデバイスを取得
            _keyboardDevice.Acquire();
            Closing += OnClosing;
            _timer = new System.Windows.Forms.Timer {Interval = 1000 / 30};
            _timer.Tick += TimerTick;
            _timer.Start();

        }


        private void OnClosing(object sender, CancelEventArgs e)
        {
            if (_timer != null)
            {
                _timer.Stop();
                _timer.Dispose();
            }
        }

        /// <summary>
        /// キーボードの入力を取得する
        /// デバイスをキャプチャできないとnullを返すので注意
        /// </summary>
        /// <returns></returns>
        public static KeyboardState GetKeyboard(Device keyboardDevice)
        {
            try
            {
                // 押されたキーをキャプチャ
                return keyboardDevice.GetCurrentKeyboardState();
            }
            catch (DirectXException ex1)
            {
                System.Diagnostics.Debug.WriteLine(ex1.ToString());
                try
                {
                    // キャプチャするデバイスを取得
                    keyboardDevice.Acquire();

                    // 押されたキーをキャプチャ
                    return keyboardDevice.GetCurrentKeyboardState();
                }
                catch (DirectXException ex2)
                {
                    System.Diagnostics.Debug.WriteLine(ex2.ToString());
                }
            }

            return null;
        }


        private void TimerTick(object sender, EventArgs e)
        {
            var keyboardState = GetKeyboard(_keyboardDevice);
            TextBlockKeyboard.Text = $"Up = {keyboardState[Key.Up]},Left = {keyboardState[Key.Left]},Right = {keyboardState[Key.Right]},Down = {keyboardState[Key.Down]},";
            GraphicsDeviceControl.Draw();
        }

    }
}
