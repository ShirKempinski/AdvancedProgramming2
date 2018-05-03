using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ImageService
{
    public static class CommandCentral
    {
        static Dictionary<string, CommandEnum> stringToCommand;
        static Dictionary<CommandEnum, ICommand> enumToCommand;
        static Mutex mtx = new Mutex();

        public static void SetCommands(ILogging logger)
        {
            string[] names = {"GetConfig", "Log", "Close"};
            CommandEnum[] enums = { CommandEnum.GetConfigCommand, CommandEnum.LogCommand, CommandEnum.CloseCommand };
            ICommand[] commands = { new GetConfigCommand(), new LogCommand(logger), new CloseCommand() };

            for (int i = 0; i < names.Length; i++)
            {
                mtx.WaitOne();
                stringToCommand.Add(names[i], enums[i]);
                enumToCommand.Add(enums[i], commands[i]);
                mtx.ReleaseMutex();
            }
        }

        public static void AddCommand(string commandName, CommandEnum @enum, ICommand command)
        {
            mtx.WaitOne();
            stringToCommand.Add(commandName, @enum);
            enumToCommand.Add(@enum, command);
            mtx.ReleaseMutex();
        }

        public static ICommand getCommand(CommandEnum cEnum)
        {
            return enumToCommand[cEnum];
        }

        public static CommandEnum getCommandEnum(string commandName)
        {
            return stringToCommand[commandName];
        }


    }
}
