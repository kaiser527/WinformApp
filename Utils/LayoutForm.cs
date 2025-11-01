using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace WinFormApp.Utils
{
    public static class LayoutForm
    {
        public static void RenderPagination(
            Panel paginatePanel,
            int currentPage,
            int totalPages,
            Action<int> onPageChanged
        )
        {
            paginatePanel.Controls.Clear();
            List<Button> btnList = new List<Button>();

            void AddButton(string text, bool enabled, Action onClick, bool highlight = false)
            {
                Button btn = new Button()
                {
                    Text = text,
                    Enabled = enabled,
                    Width = 32,
                    Height = 32,
                    AutoSize = false,
                    TextAlign = ContentAlignment.MiddleCenter,
                    BackColor = highlight ? Color.LightSkyBlue : SystemColors.Control,
                    FlatStyle = FlatStyle.Flat,
                    ForeColor = Color.Black
                };

                btn.FlatAppearance.BorderSize = 0;

                // Only trigger page change — DO NOT call loadAction here
                btn.Click += (s, e) => onClick();

                // Round shape
                btn.Paint += (s, e) =>
                {
                    var gp = new System.Drawing.Drawing2D.GraphicsPath();
                    gp.AddEllipse(0, 0, btn.Width - 1, btn.Height - 1);
                    btn.Region = new Region(gp);
                };

                Color defaultColor = btn.BackColor;
                Color hoverColor = highlight ? Color.DeepSkyBlue : Color.LightGray;

                btn.MouseEnter += (s, e) => { if (btn.Enabled) btn.BackColor = hoverColor; };
                btn.MouseLeave += (s, e) => btn.BackColor = highlight ? Color.LightSkyBlue : defaultColor;

                btnList.Add(btn);
            }

            AddButton("<<", currentPage > 1, () => onPageChanged(1));
            AddButton("<", currentPage > 1, () => onPageChanged(currentPage - 1));

            int maxButtons = 5;
            int start = Math.Max(1, currentPage - 2);
            int end = Math.Min(totalPages, start + maxButtons - 1);

            if (end - start < maxButtons - 1)
                start = Math.Max(1, end - maxButtons + 1);

            if (start > 2) AddButton("...", false, () => { });

            for (int i = start; i <= end; i++)
            {
                int page = i;
                AddButton(page.ToString(), true, () => onPageChanged(page), page == currentPage);
            }

            if (end < totalPages - 1) AddButton("...", false, () => { });

            AddButton(">", currentPage < totalPages, () => onPageChanged(currentPage + 1));
            AddButton(">>", currentPage < totalPages, () => onPageChanged(totalPages));

            int totalWidth = btnList.Sum(b => b.Width) + (btnList.Count - 1) * 5;
            int startX = (paginatePanel.Width - totalWidth) / 2;
            int x = Math.Max(startX, 5);

            foreach (var btn in btnList)
            {
                btn.Location = new Point(x, (paginatePanel.Height - btn.Height) / 2);
                paginatePanel.Controls.Add(btn);
                x += btn.Width + 5;
            }
        }
    }
}
