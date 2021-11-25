using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace winmineTools
{
    public partial class FormMain : Form
    {
        Mem memory = new Mem();
        Mem.Rect mineRect = new Mem.Rect();
        public FormMain()
        {
            InitializeComponent();            
        }
        private void btnStart_Click(object sender, EventArgs e)
        {
            memory.OpenProcess("winmine");
            if (memory.theProc==null || memory.pHandle==null)
            {
                MessageBox.Show("扫雷(winmine)游戏未运行！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            int addBase = 0x01005361;
            int flag_00 = 0;
            int flag_10 = 0x10;//16进制10,0F,8F --> 边界,安全,雷
            int flag_0F = 0x0F;
            int[,] mineInfos = new int[32, 32];            
            for (int i = 0; i < 32; i++)
            {
                int index=0;
                for (int j = 0; j < 32; j++)
                {
                    int code = addBase + 32 * i + j;
                    int mineTemp = memory.ReadByte(code.ToString("x8"));
                    if (mineTemp == flag_10)
                        break;
                    mineInfos[i, j] = mineTemp;
                    index++;
                }
                if (index == 0)
                    break;
            }
            memory.SetFocus(1);
            memory.GetWindowSize(out mineRect);
            int topBase = 100;
            int leftBase = 14;
            int mineBaseSize = 16;
            memory.CloseProcess();
            for (int i = 0; i < 32; i++)
            {
                if (mineInfos[i, 0] == flag_00 || mineInfos[i, 0] == flag_10)
                    break;
                for (int j = 0; j < 32; j++)
                {
                    if (mineInfos[i, j] == flag_00 || mineInfos[i, 0] == flag_10)
                        break;
                    if (mineInfos[i,j]==flag_0F)
                    {
                        int dx = mineRect.Left + leftBase + mineBaseSize / 2 + mineBaseSize * j;
                        int dy = mineRect.Top + topBase + mineBaseSize / 2 + mineBaseSize * i;
                        memory.MoveMouseToPoint(dx,dy);
                        memory.MouseLeftClick();
                        //System.Threading.Thread.Sleep(100);
                    }
                }
            }
        }
    }
}
