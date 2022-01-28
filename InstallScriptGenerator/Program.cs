using System.Diagnostics;


string[] separatingStrings = { "&&", "" };

int addedCount = 0;
int removedCount = 0;

if (!File.Exists(@"install.bat"))
{
    await File.WriteAllTextAsync(@"install.bat", "");
}

string ScriptFile = await File.ReadAllTextAsync(@"install.bat");
string[] commands = ScriptFile.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);
List<string> cmdList = new List<string>();
cmdList.AddRange(commands);

GenerateHeader();
Console.Write("Press any key to continue...");
Console.WriteLine();
Console.CursorVisible = false;
Console.ReadKey();
Console.CursorVisible = true;
Console.Clear();
while (true)
{
    Console.ForegroundColor = ConsoleColor.White;
    Console.WriteLine("The install.bat script currently includes the following commands:");
    Console.ResetColor();
    foreach (var cmd in cmdList)
    {
        if (cmd.Contains("npm"))
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
        }
        if (cmd.Contains("code"))
        {
            Console.ForegroundColor = ConsoleColor.Blue;
        }

        Console.WriteLine($"{cmd.TrimStart()}");

        Console.ResetColor();
    }
    Console.WriteLine("[1]: add new command | [2]: remove command | [3]: commit changes | [xx]: exit");
    var choice = Console.ReadLine();

    if (choice != null)
    {
        Console.CursorTop--;
        Console.CursorLeft = choice.Length;
        Console.Write("");
        for (int i = 0; i < choice.Length; i++)
        {
            Console.Write("   ");
        }
    }

    if (choice == "1")
    {
        while (true)
        {
            Console.WriteLine("\rEnter xx to go back");
            Console.Write("Enter the command you want to add: ");
            Console.ForegroundColor = ConsoleColor.Magenta;
            var newcmd = Console.ReadLine();
            Console.ResetColor();
            if (newcmd == "xx")
            {
                Console.Clear();
                break;
            }

            if (newcmd != null)
            {
                Console.CursorTop--;
                Console.CursorLeft = newcmd.Length;
                Console.Write("\r                                   ");
                for (int i = 0; i < newcmd.Length; i++)
                {
                    Console.Write(" ");
                }
            }
            if (newcmd != null && newcmd != string.Empty)
            {
                Console.Write("\r");
                Console.Write($"You entered the command: [");
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.Write($"{newcmd}");
                Console.ResetColor();
                Console.Write($"]");
                Console.WriteLine();
                Console.WriteLine($"Are you sure you want to add this command? [Y/N]");
                while (true)
                {
                    var IsSure = Console.ReadLine();
                    if (IsSure != null)
                    {
                        Console.CursorTop--;
                        Console.CursorLeft = IsSure.Length;
                        for (int i = 0; i < IsSure.Length; i++)
                        {
                            Console.Write("\b \b");
                        }

                    }
                    if (IsSure == "N" || IsSure == "n")
                    {
                        Console.Clear();
                        break;
                    }
                    if (IsSure == "Y" || IsSure == "y")
                    {
                        newcmd = newcmd.Replace(Environment.NewLine, "");
                        cmdList.Add(newcmd);
                        await WriteScript(cmdList);
                        addedCount += 1;
                        Console.Clear();
                        Console.BackgroundColor = ConsoleColor.White;
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("command added!");
                        Console.ResetColor();
                        break;
                    }
                }
                break;
            }
        }
    }
    if (choice == "2")
    {
        Console.Clear();
        while (true)
        {
            Console.WriteLine();
            for (int i = 0; i < cmdList.Count; i++)
            {
                string trimmedCmd = cmdList[i].TrimStart();

                if (trimmedCmd.Contains("npm"))
                {
                    Console.ForegroundColor = ConsoleColor.Magenta;
                }
                if (trimmedCmd.Contains("code"))
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                }
                Console.WriteLine($"[{i}]: {trimmedCmd}");
                Console.ResetColor();
            }
            Console.WriteLine();
            Console.WriteLine("Enter xx to go back");
            Console.Write("Enter the number of the command to remove: ");
            var ParseMe = Console.ReadLine();
            var ISInt = int.TryParse(ParseMe, out var RmCmd);
            if (ISInt && RmCmd < cmdList.Count)
            {
                Console.Write("\r");
                Console.Write($"Are you sure you want to remove the command: [");
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.Write($"{cmdList[RmCmd]}");
                Console.ResetColor();
                Console.Write($"] [Y/N]");
                Console.WriteLine();
                while (true)
                {
                    var IsSure = Console.ReadLine();
                    if (IsSure != null)
                    {
                        Console.CursorTop--;
                        Console.CursorLeft = IsSure.Length;
                        for (int i = 0; i < IsSure.Length; i++)
                        {
                            Console.Write("\b \b");
                        }

                    }
                    if (IsSure == "N" || IsSure == "n")
                    {
                        Console.Clear();
                        break;
                    }
                    if (IsSure == "Y" || IsSure == "y")
                    {
                        cmdList.RemoveAt(RmCmd);
                        await WriteScript(cmdList);
                        removedCount += 1;
                        Console.Clear();
                        Console.BackgroundColor = ConsoleColor.White;
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("command removed!");
                        Console.ResetColor();
                        break;
                    }
                }
                break;
            }
            if (ParseMe == "xx")
            {
                Console.Clear();
                break;
            }
            if (!ISInt && ParseMe != "xx")
            {
                Console.Clear();
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("You must enter a number...");
                Console.ResetColor();
            }            
            if (ISInt && RmCmd >= cmdList.Count)
            {
                Console.Clear();
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"No command with the number [{RmCmd}] exists...");
                Console.ResetColor();
            }
        }
    }
    if (choice == "3")
    {
        await CommitChangesAsync();
    }
    if (choice == "xx")
    {
        break;
    }
    if(choice != "1" && choice != "2" && choice != "3" && choice != "xx")
    {
        Console.Clear();
    }
}



static Task<int> RunProcessAsync(string fileName, string arg)
{
    var tcs = new TaskCompletionSource<int>();

    var process = new Process
    {
        StartInfo = { FileName = fileName, Arguments = arg },
        EnableRaisingEvents = true
    };
    process.StartInfo.EnvironmentVariables["GIT_ASK_YESNO"] = "false";
    
    process.Exited += (sender, args) =>
    {
        tcs.SetResult(process.ExitCode);
        process.Dispose();
    };

    process.Start();

    return tcs.Task;
}



async Task CommitChangesAsync()
{
    string gitCommand = "git";
    string gitLfsInstall = "lfs install";
    string gitLfsTrack = @"lfs track ""*.exe""";
    string gitLfsmigrate = @"lfs migrate import --include=""*.exe""";
    string gitAddArgument = @"add -A";
    string gitVersionArgument = @"--version";
    string gitCommitArgument = @$"commit -m ""added {addedCount} new and removed {removedCount} commands from the install script""";
    string gitPushArgument = @"push";
    try
    {
        Console.Clear();
        Console.BackgroundColor = ConsoleColor.White;
        Console.ForegroundColor = ConsoleColor.Green;
        Process.Start(gitCommand, gitAddArgument);
        Process.Start(gitCommand, gitCommitArgument);
        Console.ResetColor();
        //using PowerShell powershell = PowerShell.Create();
        //powershell.AddScript(@"git --version");
        //powershell.AddScript(@$"git commit -m 'added {addedCount} new and removed {removedCount} commands from the install script'");

        //var results = await powershell.InvokeAsync();

        //if (powershell.Streams.Error.Count > 0)
        //{
        //    throw powershell.Streams.Error[0].Exception;
        //}
        Console.Clear();
        Console.BackgroundColor = ConsoleColor.White;
        Console.ForegroundColor = ConsoleColor.DarkGreen;
        await RunProcessAsync(gitCommand, gitVersionArgument);
        await RunProcessAsync(gitCommand, gitLfsInstall);
        await RunProcessAsync(gitCommand, gitLfsTrack);
        await RunProcessAsync(gitCommand, gitLfsmigrate);
        await RunProcessAsync(gitCommand, gitAddArgument);
        await RunProcessAsync(gitCommand, gitCommitArgument);
        await RunProcessAsync(gitCommand, gitPushArgument);
        Console.ResetColor();
    }
    catch (Exception e)
    {
        Console.Clear();
        Console.BackgroundColor = ConsoleColor.White;
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(e.Message);
        Console.ResetColor();
        Console.WriteLine("press any key to continue...");
        Console.ReadKey();
        Console.Clear();
    }
}


async Task<List<string>> TrimmedList(List<string> inList)
{
    List<string> outlist = new List<string>();
    await Task.Delay(1);
    foreach (var str in inList)
    {
        var newstr = str.TrimStart().TrimEnd();
        outlist.Add(newstr);
    }
    return outlist;
}


async Task WriteScript(List<string> cmds)
{
    var CommandList = await TrimmedList(cmds);

    string oneLiner = string.Join(" && ", CommandList);
    string result = oneLiner.Replace(Environment.NewLine, "");

    await File.WriteAllTextAsync("install.bat", result);
}


void GenerateHeader()
{
    Console.BackgroundColor = ConsoleColor.White;
    Console.ForegroundColor = ConsoleColor.DarkMagenta;
    Console.Write(" InstallScriptGenerator ");
    Console.ForegroundColor = ConsoleColor.Black;
    Console.Write("[v1.0]               win-x64 ");
    Console.BackgroundColor = ConsoleColor.DarkRed;
    Console.Write("  ");
    Console.BackgroundColor = ConsoleColor.White;
    Console.WriteLine();
    Console.ForegroundColor = ConsoleColor.DarkBlue;
    Console.Write("                                                     ");
    Console.BackgroundColor = ConsoleColor.DarkRed;
    Console.Write("  ");
    Console.BackgroundColor = ConsoleColor.White;
    Console.WriteLine();
    Console.Write(" https://github.com/Pingu1337/InstallScriptGenerator ");
    Console.BackgroundColor = ConsoleColor.DarkRed;
    Console.Write("  ");
    Console.BackgroundColor = ConsoleColor.White;
    Console.WriteLine();
    Console.Write("                                                     ");
    Console.BackgroundColor = ConsoleColor.DarkRed;
    Console.Write("  ");
    Console.BackgroundColor = ConsoleColor.White;
    Console.WriteLine();
    Console.ResetColor();
    Console.WriteLine();
}