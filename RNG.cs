using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace PrickRand
{
    class RNG
    {
        static int threadDelay = 50;
        public string activeModeName="None";
        private volatile bool _shouldStop=false;
        string processName;
        int chaosid;
        int baseAddress;
        int time;
        bool isLoading = false;
        bool restoreRequest = false;

        public void DoWork()
        {
            while (!_shouldStop)
            {
                if(!isLoading)
                {
                    if (chaosid == 0)
                        DoNothing();
                    else if (chaosid == 1)
                        ModeInvertedControls(processName, baseAddress, time);
                    else if (chaosid == 2)
                        ModeHighFov(processName, baseAddress, time);
                    else if (chaosid == 3)
                        ModeConsoleFOV(processName, baseAddress, time);
                    else if (chaosid == 4)
                        ModeGoldeneye(processName, baseAddress, time);       
                }
                System.Threading.Thread.Sleep(threadDelay);
            }
        }

        public void SendLoadingValue(bool loading)
        {
            isLoading = loading;
        }

        public void RestoreChangesRequest()
        {
            restoreRequest = true;
        }

        public void RestoreChangesCancelValue()
        {
            restoreRequest = false;
        }

        public void ChangeSettings(int _chaosid, string _processName, int _baseAddress, int _time)
        {
            chaosid = _chaosid;
            processName = _processName;
            baseAddress = _baseAddress;
            time = _time;
        }

        public void RequestStop()
        {
            _shouldStop = true;
        }

        private void DoNothing()
        {
            activeModeName = "None";
            System.Threading.Thread.Sleep(150);
        }

        private void ModeInvertedControls(string processName, int baseAddress, int time)
        {
            activeModeName = "Inverted Controls";

            int SensitivityXAddress = 0x709E38;
            int SensitivityYAddress = 0x709E3C;
            float readSenX, readSenY;
            float sensetivityX;
            float sensetivityY;
            float originalSensetivityX = 0;
            float originalSensetivityY = 0;

            originalSensetivityX = Trainer.ReadFloat(processName, baseAddress + SensitivityXAddress);
            originalSensetivityY = Trainer.ReadFloat(processName, baseAddress + SensitivityYAddress);

            sensetivityX = -(originalSensetivityX);
            sensetivityY = -(originalSensetivityY);

            while (time != 0 && !restoreRequest)
            {
                if (isLoading)
                {
                    System.Threading.Thread.Sleep(100);
                    continue;
                }

                readSenX = Trainer.ReadFloat(processName, baseAddress + SensitivityXAddress);
                readSenY = Trainer.ReadFloat(processName, baseAddress + SensitivityYAddress);

                if (readSenX != sensetivityX || readSenY != sensetivityY)
                {
                    Trainer.WriteFloat(processName, baseAddress + SensitivityXAddress, sensetivityX);
                    Trainer.WriteFloat(processName, baseAddress + SensitivityYAddress, sensetivityY);
                }
                time--;
                System.Threading.Thread.Sleep(threadDelay);
            }

            Trainer.WriteFloat(processName, baseAddress + SensitivityXAddress, originalSensetivityX);
            Trainer.WriteFloat(processName, baseAddress + SensitivityYAddress, originalSensetivityY);
            Trace.WriteLine("Restoring original sensitivity");
        }

        void ModeHighFov(string processName, int baseAddress, int time)
        {
            activeModeName = "High FOV";

            int fovAddress = 0x1855954;
            int readFov;
            int originalFov;
            int fov = 160;

            originalFov = Trainer.ReadInteger(processName, baseAddress + fovAddress);

            while (time != 0 && !restoreRequest)
            {
                if (isLoading)
                {
                    System.Threading.Thread.Sleep(100);
                    continue;
                }

                readFov = Trainer.ReadInteger(processName, baseAddress + fovAddress);
                
                if (readFov != fov)
                {
                    Trainer.WriteInteger(processName, baseAddress + fovAddress, fov);
                }
                time--;
                System.Threading.Thread.Sleep(threadDelay);
            }
            Trainer.WriteInteger(processName, baseAddress + fovAddress, originalFov);
            Trace.WriteLine("Restoring original FOV value");
            System.Threading.Thread.Sleep(threadDelay);
        }

        void ModeConsoleFOV(string processName, int baseAddress, int time)
        {
            activeModeName = "Console vision";

            int fovAddress = 0x1855954;
            int readFov;
            int originalFov;
            int fov = 55;

            originalFov = Trainer.ReadInteger(processName, baseAddress + fovAddress);

            while (time != 0 && !restoreRequest)
            {
                if (isLoading)
                {
                    System.Threading.Thread.Sleep(100);
                    continue;
                }

                readFov = Trainer.ReadInteger(processName, baseAddress + fovAddress);

                if (readFov != fov)
                {
                    Trainer.WriteInteger(processName, baseAddress + fovAddress, fov);
                }
                time--;
                System.Threading.Thread.Sleep(threadDelay);
            }
            Trainer.WriteInteger(processName, baseAddress + fovAddress, originalFov);
            Trace.WriteLine("Restoring original FOV value");
            System.Threading.Thread.Sleep(threadDelay);
        }

        void ModeGoldeneye(string processName, int baseAddress, int time)
        {
            activeModeName = "007: Goldeneye";

            int sensentivityYAdresss = 0x709E3C;
            float readYSensetivity, originalYSensetivity;

            int lookAngleYAdresss = 0x015DE1A8;
            int[] lookAngleOffsets = new int[] { 0x14, 0x98, 0x30 };
            float readYAngle;
            float forceYAngle=-0.81f;

            originalYSensetivity = Trainer.ReadFloat(processName, baseAddress + sensentivityYAdresss);

            while (time != 0 && !restoreRequest)
            {
                if (isLoading)
                {
                    System.Threading.Thread.Sleep(100);
                    continue;
                }

                readYSensetivity = Trainer.ReadFloat(processName, baseAddress + sensentivityYAdresss);
                if (readYSensetivity != 0.0f)
                {
                    Trainer.WriteFloat(processName, baseAddress + sensentivityYAdresss, 0.0f);
                }

                readYAngle = Trainer.ReadPointerFloat(processName, baseAddress + lookAngleYAdresss, lookAngleOffsets);
                if (readYAngle != forceYAngle)
                {
                    Trainer.WritePointerFloat(processName, baseAddress + lookAngleYAdresss, lookAngleOffsets, forceYAngle);
                }
                time--;
                System.Threading.Thread.Sleep(threadDelay);
            }
            Trainer.WriteFloat(processName, baseAddress + sensentivityYAdresss, originalYSensetivity);
            Trace.WriteLine("Restoring original FOV value");
            System.Threading.Thread.Sleep(threadDelay);
        }
    }
}
