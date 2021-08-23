using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagementUI.Functionality.Executable;

namespace Elevate
{
    class Program
    {
        public static void Main(string[] args)
        {
            if (null == args || args.Length <= 0)
                throw new ArgumentNullException(nameof(args));

            if (!Parser.Initialize())
                throw new ArgumentNullException(nameof(Parser));

            List<ArgumentEntry> entries = new List<ArgumentEntry>(args.Length);

            Array.ForEach(args, (s) =>
            {
                entries.Add((ArgumentEntry)s);
                Console.WriteLine(entries.Count + " " + s);
            });

            ProcessStartInfo psi = NewStartInfo(Parser.Library, entries);
            psi.Verb = "RunAs";

            using (var proc = new Process
            {
                StartInfo = psi
            })
            {
                Console.WriteLine(proc.Start());
            }

            Console.ReadKey();
        }

        private static ProcessStartInfo NewStartInfo(ArgumentLibrary library, List<ArgumentEntry> arguments)
        {
            var psi = new ProcessStartInfo();

            arguments.ForEach((ae) =>
            {
                if (library.TryGetAction(ae.Name, out Action<ProcessStartInfo, string> action))
                {
                    action(psi, ae.Value);
                }
            });

            psi.ErrorDialog = true;
            return psi;
        }
    }
}
