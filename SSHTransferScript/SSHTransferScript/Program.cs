using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Text;
using System.Threading.Tasks;
using WinSCP;

namespace SSHTransferScript
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Enter Password for connection:");
            char[] password = new char[35];
            int passwordindex = 0;
            char input = '\0';

            while(true)
            {
                input = Console.ReadKey(true).KeyChar;

                if (input == 10 || input == 13)
                    break;

                password[passwordindex] = input;
                passwordindex++;

                if (passwordindex > 30)
                    throw new Exception("Password error");
            }

            password[passwordindex] = '\0';

            string Password =  new string(password);

            Console.WriteLine();
            try
            {
                SessionOptions sessionOptions = new SessionOptions
                {
                    Protocol = Protocol.Sftp,
                    HostName = "192.168.0.10",
                    UserName = "pi",
                    Password = Password,
                    SshHostKeyFingerprint = "ssh-ed25519 255 adEE1YiHJUEcL3Qw57CXGLs20hRJFP/ArlftWYgA/aA="
                };

                using (Session session = new Session())
                {
                    //Connect
                    session.Open(sessionOptions);

                    //Run PowerShell
                    //RunspaceConfiguration runspaceConfiguration = RunspaceConfiguration.Create();
                    //Runspace runspace = RunspaceFactory.CreateRunspace(runspaceConfiguration);
                    //runspace.Open();
                    //RunspaceInvoke scriptInvoker = new RunspaceInvoke(runspace);
                    //
                    //Pipeline pipeline = runspace.CreatePipeline();
                    //
                    //Command myCommand = new Command(Directory.GetCurrentDirectory() + @"\Powershellscript.ps1");
                    //CommandParameter testParam = new CommandParameter("session", session);
                    //myCommand.Parameters.Add(testParam);
                    //
                    //pipeline.Commands.Add(myCommand);
                    //
                    //var results = pipeline.Invoke();
                    //
                    //Console.WriteLine(results);

                    ProcessStartInfo startInfo = new ProcessStartInfo();
                    startInfo.FileName = "powershell.exe";
                    startInfo.Arguments = Directory.GetCurrentDirectory() + @"\Powershellscript.ps1";
                    startInfo.RedirectStandardOutput = true;
                    startInfo.RedirectStandardError = true;
                    startInfo.UseShellExecute = false;
                    startInfo.CreateNoWindow = true;
                    Process process = new Process();
                    process.StartInfo = startInfo;
                    process.Start();
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message + "\n" + e.StackTrace);
            }

            finally
            {
                Console.WriteLine("Script done");
                Console.ReadKey();
            }

        }
    }
}
