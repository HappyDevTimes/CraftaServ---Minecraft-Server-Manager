using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PortforwardWPF;
using System.Diagnostics;
using System.IO;
using static PortforwardWPF.MainWindow;

namespace PortforwardWPF.Classes
{
    public class Commands
    {
        MainWindow mw;
        public Commands()
        {
            mw = MainWindow.instance;
        }


        /// <summary>
        /// Bans a player by its username.
        /// </summary>
        /// <param name="user">A online players username.</param>
        public void BanUser(string user)
        {
            StreamWriter sw = mw.process.StandardInput;
            Player player = ((Player)mw.lvUsers.SelectedItem);
            sw.WriteLine("ban " + player.Username);
        }

        /// <summary>
        /// OP's a player by given username.
        /// </summary>
        /// <param name="user">A online players username.</param>
        public void OpUser()
        {
            StreamWriter sw = mw.process.StandardInput;
            Player currentUser = ((Player)mw.lvUsers.SelectedItem);
            sw.WriteLine("op " + currentUser.Username);
        }

        /// <summary>
        /// Used to give a player x amount of x item.
        /// </summary>
        /// <param name="user">A online players username.</param>
        /// <param name="item">the item ID.</param>
        /// <param name="amount">How much of this item would you like to give the player?</param>
        public void GiveItem(string user, int item, int amount)
        {

        }

        /// <summary>
        /// Teleports userOne to userTwo's location.
        /// </summary>
        /// <param name="userOne">The user you want to teleport.</param>
        /// <param name="userTwo">The user you want the first user to teleport to.</param>
        public void TeleportUser(string TeleportTo)
        {
            StreamWriter sw = mw.process.StandardInput;
            Player player = ((Player)mw.lvUsers.SelectedItem);
            sw.WriteLine("tp " + player.Username + " " + TeleportTo);
        }
    }
}
