using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;

public static class UIStyles
{
    [DllImport("gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
    public static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect,
        int nBottomRect, int nWidthEllipse, int nHeightEllipse);

    public static void RoundPanel(Panel panel, int radius)
    {
        panel.Region = Region.FromHrgn(CreateRoundRectRgn(
            0, 0, panel.Width, panel.Height, radius, radius));
    }

    public static void ModernUIButton(Button btn, Color baseColor, Color hoverColor)
    {
        btn.FlatStyle = FlatStyle.Flat;
        btn.FlatAppearance.BorderSize = 0;
        btn.BackColor = baseColor;
        btn.ForeColor = Color.White;
        btn.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        btn.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, btn.Width, btn.Height, 18, 18));
        btn.Cursor = Cursors.Hand;
        btn.FlatAppearance.MouseOverBackColor = hoverColor;
        btn.FlatAppearance.MouseDownBackColor = hoverColor;

        btn.MouseEnter += (s, e) => btn.BackColor = hoverColor;
        btn.MouseLeave += (s, e) => btn.BackColor = baseColor;
    }

    public static void RoundListView(ListView listView, int radius)
    {
        listView.BorderStyle = BorderStyle.None; // Remove border
        listView.Region = Region.FromHrgn(CreateRoundRectRgn(
            0, 0, listView.Width, listView.Height, radius, radius));
    }
}
