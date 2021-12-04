using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace kyodaiRPG
{
    public partial class FormMain : Form
    {
        Mem memory = new Mem();
        Mem.Rect mineRect = new Mem.Rect();
        int topBase = 182;
        int leftBase = 15;
        int mineBaseSizex = 31;
        int mineBaseSizey = 35;
        int addBaseQiZiShu = 0x00184DC0;
        int addBaseJiShi = 0x00186AA8;
        int addBase = 0x00199F5C;
        int[,] QiZiInfos = new int[11, 19];//11行,19列
        long QiZiShu;
        long QiZiShuNew;
        public FormMain()
        {
            InitializeComponent();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            memory.OpenProcess("kyodaiRPG");
            if (memory.theProc == null || memory.pHandle == null)
            {
                MessageBox.Show("连连看(kyodaiRPG)游戏未运行！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            bool flag = true;
            do
            {
                clearQiZiOnce();
                if (QiZiShuNew == 0 || QiZiShu == QiZiShuNew)
                {
                    flag = false;
                }
            } while (flag);
            memory.CloseProcess();
        }
        private void clearQiZiOnce()
        {
            QiZiShu = memory.ReadLong(addBaseQiZiShu.ToString("x8"));
            QiZiInfos = new int[11, 19];//11行,19列
            for (int i = 0; i < 11; i++)
            {
                for (int j = 0; j < 19; j++)
                {
                    int code = addBase + 19 * i + j;
                    QiZiInfos[i, j] = memory.ReadByte(code.ToString("x8"));
                }
            }
            memory.SetFocus(1);
            memory.GetWindowSize(out mineRect);
            //memory.CloseProcess();
            for (int x1 = 0; x1 < 11; x1++)
            {
                for (int y1 = 0; y1 < 19; y1++)
                {
                    for (int x2 = 0; x2 < 11; x2++)
                    {
                        for (int y2 = 0; y2 < 19; y2++)
                        {
                            if (canConnect(x1, y1, x2, y2))
                            {
                                clearQiZiClick(x1, y1, x2, y2);
                            }
                        }
                    }
                }
            }
            QiZiShuNew = memory.ReadLong(addBaseQiZiShu.ToString("x8"));
        }
        private void clearQiZiClick(int x1, int y1, int x2, int y2)
        {
            int dx1 = mineRect.Left + leftBase + mineBaseSizex / 2 + mineBaseSizex * y1;
            int dy1 = mineRect.Top + topBase + mineBaseSizey / 2 + mineBaseSizey * x1;
            int dx2 = mineRect.Left + leftBase + mineBaseSizex / 2 + mineBaseSizex * y2;
            int dy2 = mineRect.Top + topBase + mineBaseSizey / 2 + mineBaseSizey * x2;
            memory.MoveMouseToPoint(dx1, dy1);
            memory.MouseLeftClick();
            System.Threading.Thread.Sleep(20);
            memory.MoveMouseToPoint(dx2, dy2);
            memory.MouseLeftClick();
            System.Threading.Thread.Sleep(20);
            QiZiInfos[x1, y1] = 0;
            QiZiInfos[x2, y2] = 0;
        }
        private bool canConnect(int x1, int y1, int x2, int y2)
        {
            if (QiZiInfos[x1, y1] == 0 || QiZiInfos[x2, y2] == 0)
            {
                return false;
            }
            if (x1 == x2 && y1 == y2)
            {
                return false;
            }
            if (QiZiInfos[x1, y1] != QiZiInfos[x2, y2])
            {
                return false;
            }
            if (horizontalCheck(x1, y1, x2, y2))
            {
                return true;
            }
            if (verticalCheck(x1, y1, x2, y2))
            {
                return true;
            }
            if (turnOnceCheck(x1, y1, x2, y2))
            {
                return true;
            }
            if (turnTwiceCheck(x1, y1, x2, y2))
            {
                return true;
            }
            return false;
        }
        private bool horizontalCheck(int x1, int y1, int x2, int y2)
        {
            int xMin, xMax, yMin, yMax;
            xMin = x1 <= x2 ? x1 : x2;
            xMax = x1 >= x2 ? x1 : x2;
            yMin = y1 <= y2 ? y1 : y2;
            yMax = y1 >= y2 ? y1 : y2;
            if (x1 == x2 && y1 == y2)
            {
                return false;
            }
            if (x1 != x2)
            {
                return false;
            }
            if ((yMax - yMin) == 1)
            {
                return true;
            }
            for (int i = yMin + 1; i < yMax; i++)
            {
                if (QiZiInfos[x1, i] != 0)
                {
                    return false;
                }
            }
            return true;
        }
        private bool verticalCheck(int x1, int y1, int x2, int y2)
        {
            int xMin, xMax, yMin, yMax;
            xMin = x1 <= x2 ? x1 : x2;
            xMax = x1 >= x2 ? x1 : x2;
            yMin = y1 <= y2 ? y1 : y2;
            yMax = y1 >= y2 ? y1 : y2;
            if (x1 == x2 && y1 == y2)
            {
                return false;
            }
            if (y1 != y2)
            {
                return false;
            }
            if ((xMax - xMin) == 1)
            {
                return true;
            }
            for (int i = xMin + 1; i < xMax; i++)
            {
                if (QiZiInfos[i, y1] != 0)
                {
                    return false;
                }
            }
            return true;
        }
        private bool turnOnceCheck(int x1, int y1, int x2, int y2)
        {
            int cx, cy, dx, dy;
            if (x1 == x2 && y1 == y2)
            {
                return false;
            }
            if (x1 != x2 && y1 != y2)
            {
                cx = x1;
                cy = y2;
                dx = x2;
                dy = y1;
                if (QiZiInfos[cx, cy] == 0)
                {
                    if (horizontalCheck(x1, y1, cx, cy) && verticalCheck(cx, cy, x2, y2))
                    {
                        return true;
                    }
                }
                if (QiZiInfos[dx, dy] == 0)
                {
                    if (horizontalCheck(x1, y1, dx, dy) && verticalCheck(dx, dy, x2, y2))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        private bool turnTwiceCheck(int x1, int y1, int x2, int y2)
        {
            if (x1 == x2 && y1 == y2)
            {
                return false;
            }
            for (int i = 0; i < 11; i++)
            {
                for (int j = 0; j < 19; j++)
                {
                    if (QiZiInfos[i, j] != 0)
                    {
                        continue;
                    }
                    if (i != x1 && i != x2 && j != y1 && j != y2)
                    {
                        continue;
                    }
                    if ((i == x1 && j == y2) || (i == x2 && j == y1))
                    {
                        continue;
                    }
                    if (turnOnceCheck(x1, y1, i, j) && (horizontalCheck(i, j, x2, y2) || verticalCheck(i, j, x2, y2)))
                    {
                        return true;
                    }
                    if (turnOnceCheck(i, j, x2, y2) && (horizontalCheck(x1, y1, i, j) || verticalCheck(x1, y1, i, j)))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
