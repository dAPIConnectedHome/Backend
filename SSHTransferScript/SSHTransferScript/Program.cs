using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using WinSCP;

namespace SSHTransferScript
{
    class Program
    {
        static void Main(string[] Args)
        {
            #region Handel Commandline Arguments

            if (Args.Length != 2)
                throw new ArgumentException("Invalid number of arguments");

            string sourcePath = Args[0];
            string targetPath = Args[1];

            #endregion //Handel Commandline Arguments

            #region Password and certificate handling

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
            Console.Clear();

            #endregion //Password and certificate handling

            try
            {
                #region SSH session options

                SessionOptions sessionOptions = new SessionOptions
                {
                    Protocol = Protocol.Sftp,
                    HostName = "gitathome.dd-dns.de",
                    PortNumber = 61998,
                    UserName = "pi",
                    Password = Password,
                    SshHostKeyFingerprint = "ssh-ed25519 255 adEE1YiHJUEcL3Qw57CXGLs20hRJFP/ArlftWYgA/aA="
                };

                #endregion //SSH session options

                using (Session session = new Session())
                {
                    //Hook up Event
                    session.FileTransferProgress += Session_FileTransferProgress;
                    //Establish SSH connection
                    session.Open(sessionOptions);

                    #region DataTransfer

                    TransferOptions transferOptions = new TransferOptions();
                    transferOptions.TransferMode = TransferMode.Binary;
                    
                    TransferOperationResult transferResult;
                    transferResult = session.PutFiles(@sourcePath, targetPath, false, transferOptions);

                    transferResult.Check();

                    if (_lastFileName != null)
                    {
                        Console.WriteLine();
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.Write("{0}", _totalTransferedFilesCount);
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write(" Files transfered...");
                        Console.WriteLine();
                    }
                    else
                    {
                        Console.WriteLine();
                        Console.WriteLine("No Files transfered...");
                    }

                    #endregion //DataTransfer

                    #region Make Software Runnable

                    session.ExecuteCommand("chown pi " + targetPath + " -R").Check();

                    session.ExecuteCommand("chmod 777 " + targetPath + " -R").Check();

                    #endregion //Make Software Runnable

                    #region Run Software

                    Console.WriteLine("Do you want to Run the Application? (Y/n)");

                    char response = '\0';
                    while(response != 'Y')
                    {
                        response = Console.ReadKey().KeyChar;

                        if (response == 'n')
                            return;
                    }

                    session.ExecuteCommand("dotnet run " + "/home/pi/Desktop/DAPISmartHomeAppAPI/DapiSmartHomeMQTT_BackendServer");

                    #endregion //Run Software

                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message + "\n" + e.StackTrace);
            }

            finally
            {   
                Console.ReadKey();
            }

        }

        private static string _lastFileName;
        private static int _totalTransferedFilesCount = 0;
        private static void Session_FileTransferProgress(object sender, FileTransferProgressEventArgs e)
        {
            if((_lastFileName != null) && (_lastFileName != e.FileName))
            {
                Console.WriteLine();
                ++_totalTransferedFilesCount;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("\r{0:P0} ",e.OverallProgress);
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("of transfer complete\t{0} ({1:P0})", e.FileName, e.FileProgress);

            _lastFileName = e.FileName;
        }
    }
}
