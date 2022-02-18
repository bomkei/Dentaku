using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace dentaku {
  public partial class Form1 : Form {
    enum CalcKind {
      Add,
      Sub,
      Mul,
      Div
    }

    readonly string[] CalcStrings = {
      "+",
      "-",
      "*",
      "/"
    };

    List<Button> calc_buttons = new List<Button>();

    TextBox numview;
    Button last_calc_button = null;
    bool crashed = false;
    bool numchanged = false;

    [System.Runtime.InteropServices.DllImport("kernel32.dll")]
    private static extern bool AllocConsole();

    public Form1() {
      InitializeComponent();

      // ラベル削除 ( 実際は非表示にするだけ )
      this.label1.Visible = false;

      // ボタン作成 ( 1 ~ 9 )
      for (int i = 0; i < 9; i++) {
        var button = new Button();
        button.Text = $"{i + 1}";
        button.Size = new Size(48, 48);
        button.Location = new Point(24 + (i % 3) * 64, (i / 3 + 1) * 64);
        button.Click += NumButtonClick;
        button.Font = new Font("Meiryo", 14);
        this.Controls.Add(button);
      }

      // ボタン作成 ( 0 )
      var zero = new Button();
      zero.Text = "0";
      zero.Size = new Size(48, 48);
      zero.Location = new Point(24, 4 * 64);
      zero.Click += NumButtonClick;
      zero.Font = new Font("Meiryo", 14);
      this.Controls.Add(zero);

      // ボタン作成 ( + - * / )
      for (int i = 0; i < 4; i++) {
        var button = new Button();
        button.Text = CalcStrings[i];
        button.Size = new Size(48, 48);
        button.Location = new Point(24 + 3 * 64, (i + 1) * 64);
        button.Click += CalcButtonClick;
        button.Font = new Font("Meiryo", 14);
        this.Controls.Add(button);
        calc_buttons.Add(button);
      }

      // ボタン作成 ( = )
      var btn_eq = new Button();
      btn_eq.Text = "=";
      btn_eq.Size = new Size(48, 48);
      btn_eq.Location = new Point(24 + 64, 4 * 64);
      btn_eq.Click += EqualButtonClick;
      btn_eq.Font = new Font("Meiryo", 14);
      this.Controls.Add(btn_eq);

      // ボタン作成 ( C )
      var btn_clear = new Button();
      btn_clear.Text = "C";
      btn_clear.Size = new Size(48, 48);
      btn_clear.Location = new Point(24 + 2 * 64, 4 * 64);
      btn_clear.Click += ClearButtonClick;
      btn_clear.Font = new Font("Meiryo", 14);
      this.Controls.Add(btn_clear);

      // 数値表示用テキストボックス
      numview = new TextBox();
      numview.Size = new Size(64 * 4 - (64 - 48), 0);
      numview.Location = new Point(24, 16);
      numview.Font = new Font("Meiryo", 11);
      numview.ReadOnly = true;
      numview.BackColor = Color.White;
      numview.Text = "0";
      this.Controls.Add(numview);

      // ウィンドウサイズ変更
      this.Size = new Size(300, 360);


    }

    private void Error() {
      ClearAll();
      numview.Text = "Error";
      crashed = true;
    }

    private void ClearNum() {
      numview.Text = "0";
    }

    private void ClearAll() {
      foreach (var b in calc_buttons) {
        b.Tag = null;
      }

      numview.Text = "0";
      last_calc_button = null;
      crashed = false;
    }

    private void NumButtonClick(object sender, EventArgs e) {
      if (crashed || numview.Text == "0") {
        numview.Text = "";
      }

      if (last_calc_button != null && !numchanged) {
        numview.Text = "";
      }

      numchanged = true;
      numview.Text += ((Button)sender).Text;
    }

    private Int64 Calc(string kind, Int64 a, Int64 b) {
      switch (kind) {
        case "+":
          a += b;
          break;

        case "-":
          a -= b;
          break;

        case "*":
          a *= b;
          break;

        case "/":
          if (b == 0) {
            Error();
            break;
          }

          a /= b;
          break;
      }

      return a;
    }

    private void CalcButtonClick(object _sender, EventArgs e) {
      var sender = (Button)_sender;

      if (last_calc_button != null && last_calc_button != sender) {
        if (!numchanged) {
          sender.Tag = numview.Text;
          last_calc_button = sender;
          return;
        }

        Console.WriteLine($"{(string)last_calc_button.Text} {(string)last_calc_button.Tag} {numview.Text}");

        numview.Text = Calc((string)last_calc_button.Text, Convert.ToInt64((string)last_calc_button.Tag), Convert.ToInt64(numview.Text)).ToString();
        sender.Tag = null;
        Console.WriteLine("176");
      }

      last_calc_button = sender;

      if (sender.Tag == null) {
        sender.Tag = numview.Text;
        Console.WriteLine("143");
      }
      else {
        numview.Text = Calc(sender.Text, Convert.ToInt64((string)sender.Tag), Convert.ToInt64(numview.Text)).ToString();
        sender.Tag = numview.Text;
        Console.WriteLine("149");
      }

      numchanged = false;
    }

    private void EqualButtonClick(object sender, EventArgs e) {
      if (last_calc_button == null) {
        return;
      }

      numview.Text= Calc((string)last_calc_button.Text, Convert.ToInt64((string)last_calc_button.Tag), Convert.ToInt64(numview.Text)).ToString();
      last_calc_button = null;
    }
    
    private void ClearButtonClick(object sender, EventArgs e) {
      ClearAll();
    }


  }
}