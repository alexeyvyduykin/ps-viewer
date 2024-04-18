using pscli.Services;
using pscli.Utils;
using System.CommandLine;

namespace pscli.Commands;

public static class Connect
{
    public static Command CreateCommand()
    {
        var hostOption = new Option<string>(name: "--host", description: "MongoDB host.")
        {
            IsRequired = true
        };

        var usernameOption = new Option<string?>(name: "--username", description: "MongoDB username.");

        var passwordOption = new Option<string?>(name: "--password", description: "MongoDB username password.");

        var command = new Command("connect", "???") { hostOption, usernameOption, passwordOption };

        command.SetHandler(ConnectImpl, hostOption, usernameOption, passwordOption);

        return command;
    }

    private static void ConnectImpl(string host, string? username, string? password)
    {
        AppOptions.Update(options =>
        {
            string? connectionString;

            if (host == "atlas")
            {
                username ??= "user";
                password ??= "ds277fgj";

                connectionString = $"mongodb+srv://{username}:{password}@maincluster.ksegec5.mongodb.net/?retryWrites=true&w=majority&appName=MainCluster";
            }
            else
            {
                string str = "";
                if (username is { } && password is { })
                {
                    str = $"{username}:{password}@";
                }
                connectionString = $"mongodb://{str}{host}";
            }

            var psService = new PsService(connectionString);

            options.ConnectionString = connectionString;
        });
    }
}
