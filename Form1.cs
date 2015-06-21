using System;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading;


namespace PrickRand
{
    public partial class Form1 : Form
    {
        // Base address value for pointers.
        int baseAddress = 0x0000000;
        int isLoadingAdress = 0x1876708;

        RNG _rng = new RNG();

        // Other variables.
        System.Text.Encoding enc = System.Text.Encoding.UTF8;
        Process[] myProcess;
        string processName;
        Random rnd = new Random();

        public int time = 0;
        public int randomvalue=0;

        public bool ChaosEnabled = false;


        /*------------------
        -- INITIALIZATION --
        ------------------*/
        public Form1()
        {
            InitializeComponent();
            processName = "dxhr";
            Thread RNGThread = new Thread(_rng.DoWork);
            RNGThread.Start();
        }

        bool foundProcess = false;

        private void Timer_Tick(object sender, EventArgs e)
        {
            try
            {
                myProcess = Process.GetProcessesByName(processName);
                if (myProcess.Length > 0)
                {
                    if (foundProcess == false)
                        System.Threading.Thread.Sleep(100);

                    IntPtr startOffset = myProcess[0].MainModule.BaseAddress;
                    IntPtr endOffset = IntPtr.Add(startOffset, myProcess[0].MainModule.ModuleMemorySize);
                    baseAddress = startOffset.ToInt32();
                    foundProcess = true;
                }
                else
                    foundProcess = false;

                if (foundProcess)
                {
                    // The game is running, ready for memory reading.
                    LB_Running.Text = "DXHR IS RUNNING";
                    LB_Running.ForeColor = Color.Green;

                    if (Trainer.ReadByte(processName, baseAddress + isLoadingAdress) == 0)
                        _rng.SendLoadingValue(false);
                    else
                        _rng.SendLoadingValue(true);

                    if (ChaosEnabled)
                    {
                        _rng.ChangeSettings(randomvalue, processName, baseAddress, time);
                    }

                    L_ActiveMode.Text = _rng.activeModeName;
                }
                else
                {
                    // The game process has not been found, reseting values.
                    LB_Running.Text = "DXHR IS NOT RUNNING";
                    LB_Running.ForeColor = Color.Red;
                    _rng.RestoreChangesRequest();

                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Timer.Start();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            _rng.RestoreChangesRequest();
            System.Threading.Thread.Sleep(150);
            _rng.RequestStop();
        }

        private void C_EnableChaos_CheckedChanged(object sender, EventArgs e)
        {
            if (C_EnableChaos.Checked)
            {
                ChaosEnabled = true;
                _rng.RestoreChangesCancelValue();
                GenerateNewSettingsTick.Start();
            }
            else
            {
                _rng.RestoreChangesRequest();
                ChaosEnabled = false;
                randomvalue = 0;
                _rng.ChangeSettings(randomvalue, processName, baseAddress, time);
                GenerateNewSettingsTick.Stop();
            }
        }

        private void GenerateNewSettingsTick_Tick(object sender, EventArgs e)
        {
            randomvalue = rnd.Next(1, 5);
            time = 20 * (rnd.Next(15, 45));
        }
    }
}
