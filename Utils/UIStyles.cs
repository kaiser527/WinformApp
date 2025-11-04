using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;

public static class UIStyles
{
    private static readonly Color PlaceholderColor = Color.Gray;
    private static readonly Color TextColor = Color.Black;

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

    public static void ApplyPlaceholder(TextBox textBox, string placeholder, ref bool isPlaceholderFlag)
    {
        isPlaceholderFlag = true;
        textBox.ForeColor = PlaceholderColor;
        textBox.Text = placeholder;
        isPlaceholderFlag = false;

        // Attach events only once
        textBox.Enter -= RemovePlaceholder;
        textBox.Enter += RemovePlaceholder;

        textBox.Leave -= SetPlaceholder;
        textBox.Leave += SetPlaceholder;

        // Store placeholder text in Tag
        textBox.Tag = (placeholder, isPlaceholderFlag);
    }

    private static void RemovePlaceholder(object sender, EventArgs e)
    {
        var txt = sender as TextBox;
        var (placeholder, _) = ((string, bool))txt.Tag;

        if (txt.Text == placeholder)
        {
            txt.Text = "";
            txt.ForeColor = TextColor;
        }
    }

    private static void SetPlaceholder(object sender, EventArgs e)
    {
        var txt = sender as TextBox;
        var (placeholder, flag) = ((string, bool))txt.Tag;

        if (string.IsNullOrWhiteSpace(txt.Text))
        {
            flag = true;
            txt.ForeColor = PlaceholderColor;
            txt.Text = placeholder;
            flag = false;
            txt.Tag = (placeholder, flag);
        }
    }
}
