using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;

namespace GFSetupWizard.App.WinUI3.SystemIntegration
{
    public static class SystemApplicationLauncher
    {
        // Delegate for message dialogs that will be set by the application
        // Using object as return type to avoid direct dependency on WinUI types
        public static Func<string, string, Task<object>> ShowMessageDialogAsync { get; set; }

        /// <summary>
        /// Checks if OneDrive is configured on the system.
        /// </summary>
        /// <returns>True if OneDrive is configured, false otherwise.</returns>
        public static bool IsOneDriveConfigured()
        {
            try
            {
                // Check if OneDrive folder exists in user profile
                string oneDrivePath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                    "OneDrive - GlobalFoundries");
                
                // Also check for the OneDrive process running
                bool processRunning = Process.GetProcessesByName("OneDrive").Length > 0;
                
                // Consider OneDrive configured if both the folder exists and the process is running
                return Directory.Exists(oneDrivePath) && processRunning;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking OneDrive configuration: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Launches OneDrive and shows appropriate feedback based on whether OneDrive is configured or not.
        /// </summary>
        /// <returns>True if OneDrive was launched successfully, false otherwise.</returns>
        public static async Task<bool> LaunchOneDriveWithFeedbackAsync()
        {
            // Check if OneDrive is already configured
            bool isConfigured = IsOneDriveConfigured();
            
            // Launch OneDrive
            bool success = LaunchOneDrive();
            
            if (success)
            {
                // Show different messages based on whether OneDrive is configured or not
                if (isConfigured)
                {
                    await ShowMessageDialogAsync(
                        "OneDrive setup has been initiated. Allow up to one minute for it to launch. If it fails to load, click the button again or manually search for the OneDrive app to launch it",
                        "OneDrive Launched"
                    );
                }
                else
                {
                    await ShowMessageDialogAsync(
                        "OneDrive setup has been initiated. Allow up to one minute for it to launch. If it fails to load, click the button again or manually search for the OneDrive app to launch it",
                        "OneDrive Setup"
                    );
                }
            }
            else
            {
                await ShowMessageDialogAsync(
                    "Unable to launch OneDrive automatically. Please open it manually from the Start menu.",
                    "Launch Failed"
                );
            }
            
            return success;
        }

        public static bool LaunchOneDrive()
        {
            // Check if OneDrive is already configured
            bool isConfigured = IsOneDriveConfigured();
            
            try
            {
                if (isConfigured)
                {
                    // If OneDrive is configured, launch the OneDrive application directly
                    try
                    {
                        // Method 1: Try to launch OneDrive directly using the executable path
                        string oneDrivePath = Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                            "Microsoft", "OneDrive", "OneDrive.exe");
                        
                        if (File.Exists(oneDrivePath))
                        {
                            Process.Start(new ProcessStartInfo
                            {
                                FileName = oneDrivePath,
                                UseShellExecute = true
                            });
                            return true;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error launching OneDrive executable: {ex.Message}");
                    }
                    
                    // Method 2: Try to launch OneDrive using the shell command
                    try
                    {
                        ProcessStartInfo startInfo = new ProcessStartInfo
                        {
                            FileName = "explorer.exe",
                            Arguments = "shell:AppsFolder\\Microsoft.SkyDrive_8wekyb3d8bbwe!Microsoft.SkyDrive.Desktop",
                            UseShellExecute = true
                        };
                        
                        Process.Start(startInfo);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error launching OneDrive via shell: {ex.Message}");
                    }
                    
                    // Method 3: Try to launch OneDrive using the protocol handler
                    try
                    {
                        ProcessStartInfo startInfo = new ProcessStartInfo
                        {
                            FileName = "cmd.exe",
                            Arguments = "/c start onedrive:",
                            UseShellExecute = true,
                            CreateNoWindow = true
                        };
                        
                        Process.Start(startInfo);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error launching OneDrive via protocol: {ex.Message}");
                        return false;
                    }
                }
                else
                {
                    // If OneDrive is not configured, launch the OneDrive setup process
                    // Based on testing, only Method 1 (direct executable path from Program Files) works reliably
                    // for initiating the OneDrive setup process on unconfigured systems
                    
                    // Method 1: Launch OneDrive setup using the executable path in Program Files
                    try
                    {
                        string oneDriveSetupPath = Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                            "Microsoft OneDrive", "OneDrive.exe");
                        
                        if (File.Exists(oneDriveSetupPath))
                        {
                            Process.Start(new ProcessStartInfo
                            {
                                FileName = oneDriveSetupPath,
                                UseShellExecute = true
                            });
                            return true;
                        }
                        else
                        {
                            Console.WriteLine("OneDrive executable not found in Program Files.");
                            
                            // Fallback to Method 2 only if Method 1 fails due to file not found
                            string oneDriveSetupPathX86 = Path.Combine(
                                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
                                "Microsoft OneDrive", "OneDrive.exe");
                            
                            if (File.Exists(oneDriveSetupPathX86))
                            {
                                Process.Start(new ProcessStartInfo
                                {
                                    FileName = oneDriveSetupPathX86,
                                    UseShellExecute = true
                                });
                                return true;
                            }
                            else
                            {
                                Console.WriteLine("OneDrive executable not found in Program Files (x86) either.");
                                return false;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error launching OneDrive setup from Program Files: {ex.Message}");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in LaunchOneDrive: {ex.Message}");
                return false;
            }
            
            // If we reach here, all methods failed
            return false;
        }

        public static bool LaunchOutlook()
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "outlook",
                    UseShellExecute = true
                });
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error launching Outlook: {ex.Message}");
                return false;
            }
        }

        public static bool LaunchTeams()
        {
            try
            {
                // Use the shell command approach that works for Outlook
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = "explorer.exe",
                    Arguments = "shell:AppsFolder\\MSTeams_8wekyb3d8bbwe!MSTeams",
                    UseShellExecute = true
                };
                
                Process.Start(startInfo);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error launching Teams: {ex.Message}");
                
                try
                {
                    // Try alternative method using protocol handler
                    ProcessStartInfo startInfo = new ProcessStartInfo
                    {
                        FileName = "cmd.exe",
                        Arguments = "/c start msteams:",
                        UseShellExecute = true,
                        CreateNoWindow = true
                    };
                    
                    Process.Start(startInfo);
                    return true;
                }
                catch (Exception ex2)
                {
                    Console.WriteLine($"Error launching Teams (alternative method): {ex2.Message}");
                    return false;
                }
            }
        }

        public static bool LaunchSoftwareCenter()
        {
            try
            {
                // Path to Software Center executable - specifically using the SCClient.exe in ClientUX folder
                // as requested to ensure the correct version is launched
                string softwareCenterPath = @"C:\Windows\CCM\ClientUX\SCClient.exe";
                
                if (File.Exists(softwareCenterPath))
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = softwareCenterPath,
                        UseShellExecute = true
                    });
                    return true;
                }
                else
                {
                    Console.WriteLine("Software Center executable not found at expected location.");
                    
                    // Try alternative methods to launch Software Center
                    try
                    {
                        // Try using the shell command
                        ProcessStartInfo startInfo = new ProcessStartInfo
                        {
                            FileName = "cmd.exe",
                            Arguments = "/c start softwarecenter:",
                            UseShellExecute = true,
                            CreateNoWindow = true
                        };
                        
                        Process.Start(startInfo);
                        return true;
                    }
                    catch (Exception altEx)
                    {
                        Console.WriteLine($"Error launching Software Center (alternative method): {altEx.Message}");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error launching Software Center: {ex.Message}");
                
                // Try alternative methods to launch Software Center
                try
                {
                    // Try using the shell command
                    ProcessStartInfo startInfo = new ProcessStartInfo
                    {
                        FileName = "cmd.exe",
                        Arguments = "/c start softwarecenter:",
                        UseShellExecute = true,
                        CreateNoWindow = true
                    };
                    
                    Process.Start(startInfo);
                    return true;
                }
                catch (Exception altEx)
                {
                    Console.WriteLine($"Error launching Software Center (alternative method): {altEx.Message}");
                    return false;
                }
            }
        }

        public static bool OpenServicePortal()
        {
            try
            {
                // Open service portal URL in default browser
                string servicePortalUrl = "https://globalfoundries.service-now.com/esc";
                
                Process.Start(new ProcessStartInfo
                {
                    FileName = servicePortalUrl,
                    UseShellExecute = true
                });
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error opening Service Portal: {ex.Message}");
                return false;
            }
        }
        
        public static bool OpenRsaTokenRequestForVpn()
        {
            try
            {
                // Open RSA token request URL in default browser
                string rsaTokenRequestUrl = "https://globalfoundries.service-now.com/esc?id=sc_cat_item&sys_id=879725cf2b7920002c83a15069da15bf&table=sc_cat_item&searchTerm=secur";
                
                Process.Start(new ProcessStartInfo
                {
                    FileName = rsaTokenRequestUrl,
                    UseShellExecute = true
                });
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error opening RSA token request: {ex.Message}");
                return false;
            }
        }
        
        public static bool OpenSoftwareRequestPortal()
        {
            try
            {
                // Open software request URL in default browser
                string softwareRequestUrl = "https://globalfoundries.service-now.com/esc?id=sc_cat_item&sys_id=b6a36416242301005f424364fd5576e3&table=sc_cat_item&searchTerm=software";
                
                Process.Start(new ProcessStartInfo
                {
                    FileName = softwareRequestUrl,
                    UseShellExecute = true
                });
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error opening software request portal: {ex.Message}");
                return false;
            }
        }
        
        #region Native Input Methods

        // Constants for INPUT structure
        private const int INPUT_MOUSE = 0;
        private const int INPUT_KEYBOARD = 1;
        private const int INPUT_HARDWARE = 2;
        private const uint KEYEVENTF_EXTENDEDKEY = 0x0001;
        private const uint KEYEVENTF_KEYUP = 0x0002;
        private const uint KEYEVENTF_UNICODE = 0x0004;
        private const uint KEYEVENTF_SCANCODE = 0x0008;

        // Virtual key codes
        private const int VK_CONTROL = 0x11;
        private const int VK_L = 0x4C;
        private const int VK_RETURN = 0x0D;

        // INPUT structure for SendInput
        [StructLayout(LayoutKind.Sequential)]
        private struct INPUT
        {
            public int type;
            public InputUnion u;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct InputUnion
        {
            [FieldOffset(0)]
            public MOUSEINPUT mi;
            [FieldOffset(0)]
            public KEYBDINPUT ki;
            [FieldOffset(0)]
            public HARDWAREINPUT hi;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public uint mouseData;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct KEYBDINPUT
        {
            public ushort wVk;
            public ushort wScan;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct HARDWAREINPUT
        {
            public uint uMsg;
            public ushort wParamL;
            public ushort wParamH;
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

        [DllImport("user32.dll")]
        private static extern IntPtr GetMessageExtraInfo();

        #endregion

        /// <summary>
        /// Launches Edge and uses direct Windows API to automatically navigate to the sync settings page.
        /// </summary>
        /// <returns>True if successful, false otherwise.</returns>
        public static bool LaunchEdgeWithInputSimulator()
        {
            try
            {
                // Kill any running instances of Edge first
                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = "taskkill",
                        Arguments = "/F /IM msedge.exe",
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }).WaitForExit(2000);
                    
                    // Small delay to ensure processes are fully terminated
                    Thread.Sleep(1000);
                }
                catch (Exception killEx)
                {
                    Console.WriteLine($"Error killing Edge processes (non-critical): {killEx.Message}");
                    // Continue even if we couldn't kill existing processes
                }
                
                // Launch Edge with specific profile
                Process.Start(new ProcessStartInfo
                {
                    FileName = "msedge.exe",
                    Arguments = "--profile-directory=\"Profile X\" edge://settings/profiles",
                    UseShellExecute = true
                });
                
                // Give Edge time to fully initialize
                Thread.Sleep(2000);
                
                // Use direct Windows API to navigate to the correct page
                
                // Focus the address bar (Ctrl+L)
                SendKeyboardShortcut(VK_CONTROL, VK_L);
                Thread.Sleep(500);
                
                // Type the URL
                SimulateTyping("edge://settings/profiles");
                Thread.Sleep(500);
                
                // Press Enter
                SimulatePressEnter();
                
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in LaunchEdgeWithInputSimulator: {ex.Message}");
                return false;
            }
        }
        
        /// <summary>
        /// Sends a keyboard shortcut using the Windows API
        /// </summary>
        /// <param name="modifierKey">The modifier key (e.g., Ctrl, Alt, Shift)</param>
        /// <param name="key">The key to press</param>
        /// <returns>True if successful</returns>
        public static bool SendKeyboardShortcut(int modifierKey, int key)
        {
            try
            {
                // Press modifier key down
                INPUT[] inputs = new INPUT[4];
                
                // Modifier key down
                inputs[0] = new INPUT
                {
                    type = INPUT_KEYBOARD,
                    u = new InputUnion
                    {
                        ki = new KEYBDINPUT
                        {
                            wVk = (ushort)modifierKey,
                            wScan = 0,
                            dwFlags = 0,
                            time = 0,
                            dwExtraInfo = GetMessageExtraInfo()
                        }
                    }
                };
                
                // Key down
                inputs[1] = new INPUT
                {
                    type = INPUT_KEYBOARD,
                    u = new InputUnion
                    {
                        ki = new KEYBDINPUT
                        {
                            wVk = (ushort)key,
                            wScan = 0,
                            dwFlags = 0,
                            time = 0,
                            dwExtraInfo = GetMessageExtraInfo()
                        }
                    }
                };
                
                // Key up
                inputs[2] = new INPUT
                {
                    type = INPUT_KEYBOARD,
                    u = new InputUnion
                    {
                        ki = new KEYBDINPUT
                        {
                            wVk = (ushort)key,
                            wScan = 0,
                            dwFlags = KEYEVENTF_KEYUP,
                            time = 0,
                            dwExtraInfo = GetMessageExtraInfo()
                        }
                    }
                };
                
                // Modifier key up
                inputs[3] = new INPUT
                {
                    type = INPUT_KEYBOARD,
                    u = new InputUnion
                    {
                        ki = new KEYBDINPUT
                        {
                            wVk = (ushort)modifierKey,
                            wScan = 0,
                            dwFlags = KEYEVENTF_KEYUP,
                            time = 0,
                            dwExtraInfo = GetMessageExtraInfo()
                        }
                    }
                };
                
                uint result = SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(INPUT)));
                
                if (result != inputs.Length)
                {
                    Console.WriteLine($"SendInput failed with error code {Marshal.GetLastWin32Error()}");
                    return false;
                }
                
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending keyboard shortcut: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Simulates typing in a control using the Windows API
        /// </summary>
        /// <param name="text">The text to type</param>
        /// <returns>True if successful, false otherwise</returns>
        public static bool SimulateTyping(string text)
        {
            try
            {
                // Wait a moment before typing
                Task.Delay(500).Wait();
                
                // Type the text character by character
                foreach (char c in text)
                {
                    INPUT[] inputs = new INPUT[2];
                    
                    // Key down
                    inputs[0] = new INPUT
                    {
                        type = INPUT_KEYBOARD,
                        u = new InputUnion
                        {
                            ki = new KEYBDINPUT
                            {
                                wVk = 0,
                                wScan = (ushort)c,
                                dwFlags = KEYEVENTF_UNICODE,
                                time = 0,
                                dwExtraInfo = GetMessageExtraInfo()
                            }
                        }
                    };
                    
                    // Key up
                    inputs[1] = new INPUT
                    {
                        type = INPUT_KEYBOARD,
                        u = new InputUnion
                        {
                            ki = new KEYBDINPUT
                            {
                                wVk = 0,
                                wScan = (ushort)c,
                                dwFlags = KEYEVENTF_UNICODE | KEYEVENTF_KEYUP,
                                time = 0,
                                dwExtraInfo = GetMessageExtraInfo()
                            }
                        }
                    };
                    
                    uint result = SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(INPUT)));
                    
                    if (result != inputs.Length)
                    {
                        Console.WriteLine($"SendInput failed with error code {Marshal.GetLastWin32Error()}");
                        return false;
                    }
                    
                    Task.Delay(10).Wait(); // Small delay between keypresses
                }
                
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error simulating typing: {ex.Message}");
                return false;
            }
        }
        
        /// <summary>
        /// Simulates pressing the Enter key using the Windows API
        /// </summary>
        /// <returns>True if successful, false otherwise</returns>
        public static bool SimulatePressEnter()
        {
            try
            {
                INPUT[] inputs = new INPUT[2];
                
                // Enter key down
                inputs[0] = new INPUT
                {
                    type = INPUT_KEYBOARD,
                    u = new InputUnion
                    {
                        ki = new KEYBDINPUT
                        {
                            wVk = (ushort)VK_RETURN,
                            wScan = 0,
                            dwFlags = 0,
                            time = 0,
                            dwExtraInfo = GetMessageExtraInfo()
                        }
                    }
                };
                
                // Enter key up
                inputs[1] = new INPUT
                {
                    type = INPUT_KEYBOARD,
                    u = new InputUnion
                    {
                        ki = new KEYBDINPUT
                        {
                            wVk = (ushort)VK_RETURN,
                            wScan = 0,
                            dwFlags = KEYEVENTF_KEYUP,
                            time = 0,
                            dwExtraInfo = GetMessageExtraInfo()
                        }
                    }
                };
                
                uint result = SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(INPUT)));
                
                if (result != inputs.Length)
                {
                    Console.WriteLine($"SendInput failed with error code {Marshal.GetLastWin32Error()}");
                    return false;
                }
                
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error simulating Enter key press: {ex.Message}");
                return false;
            }
        }
        
        /// <summary>
        /// Copies the Edge sync settings URL to the clipboard.
        /// </summary>
        /// <returns>True if successful, false otherwise.</returns>
        public static bool CopyEdgeSyncUrlToClipboard()
        {
            try
            {
                // Set the URL to clipboard using WinUI3 clipboard API
                string syncUrl = "edge://settings/profiles/sync";
                
                var dataPackage = new DataPackage();
                dataPackage.SetText(syncUrl);
                Clipboard.SetContent(dataPackage);
                
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error copying Edge sync URL to clipboard: {ex.Message}");
                return false;
            }
        }
    }
}
