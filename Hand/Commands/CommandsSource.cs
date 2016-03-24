using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hand.Commands
{
    public class CommandsSource
    {
        public async Task SaveAsync(IEnumerable<MotorCommand> commands)
        {
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(commands.ToArray());

            var file = await Windows.Storage.ApplicationData.Current.LocalFolder.CreateFileAsync("commands.json", Windows.Storage.CreationCollisionOption.ReplaceExisting);
            await Windows.Storage.FileIO.WriteTextAsync(file, json);
        }

        public async Task<IEnumerable<MotorCommand>> LoadAsync()
        {
            var file = await Windows.Storage.ApplicationData.Current.LocalFolder.GetFileAsync("commands.json");
            string json = await Windows.Storage.FileIO.ReadTextAsync(file);

            return Newtonsoft.Json.JsonConvert.DeserializeObject<MotorCommand[]>(json);

            //if (Windows.Storage.ApplicationData.Current.LocalSettings.Values.ContainsKey("commands"))
            //{
            //    var cmds = Windows.Storage.ApplicationData.Current.LocalSettings.Values["commands"] as MotorCommand[];
            //    if (cmds != null && cmds.Any())
            //    {
            //        return cmds;
            //    }
            //}

            //return Enumerable.Empty<MotorCommand>();
        }
    }

}
