using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace Lab1_Turmit
{
    public class TurmiteRule
    {
    public char CurrentState { get; set; }
    public int OldColor { get; set; }
    public int NewColor { get; set; }
    public int Turn { get; set; }
    public char NewState { get; set; }

    public TurmiteRule(char currentState, int oldColor,
                       int newColor, int turn, char newState)
    {
        CurrentState = currentState;
        OldColor = oldColor;
        NewColor = newColor;
        Turn = turn;
        NewState = newState;
    }
    }

    public partial class Form1 : Form
    {
        private List<TurmiteRule> rules = new List<TurmiteRule>();
        private const int FieldWidth = 200;
        private const int FieldHeight = 200;
        private Color[] colors =
        {
            Color.Black,
            Color.White,
            Color.Red,
            Color.Green,
            Color.Blue,
            Color.Yellow,
            Color.Orange,
            Color.Purple,
            Color.Cyan,
            Color.Magenta,
            Color.Brown,
            Color.Gray,
            Color.Pink,
            Color.Lime,
            Color.Navy,
            Color.Gold
        };

        private const int CellSize = 2;
        private int[,] field = new int[FieldWidth, FieldHeight];
        private int turmitX;
        private int turmitY;
        private char turmiteState;
        private string turmiteDirection;
        private int iterationCount;

        private Panel fieldPanel;

        private Label iterationLabel;
        private Label stateLabel;
        private Label directionLabel;
        private Label positionLabel;

        private TextBox iterationsTextBox;
        private TextBox xTextBox;
        private TextBox yTextBox;

        private ComboBox stateComboBox;
        private ComboBox directionComboBox;

        private Button nextStepButton;
        private Button automaticButton;
        private Button resetButton;
        private Button saveButton;
        private Button pauseButton;
        private Button executeButton;
        private Button applyButton;
        private Button rulesButton;
        private System.Windows.Forms.Timer automaticTimer;
        private bool isAutomaticRunning = false;
        private int automaticTargetIterations = 100;
        private int automaticDelay = 100;

        public Form1()
        {
            InitializeComponent();
            CreateInterface();
            CreateRules(); // створення правил
            
            ResetAutomaton(); // початковий стан

            automaticTimer = new System.Windows.Forms.Timer();
            automaticTimer.Interval = automaticDelay;
            automaticTimer.Tick += AutomaticTimer_Tick;
        }

        private void AutomaticTimer_Tick(object sender, EventArgs e)
        {
            if (iterationCount < automaticTargetIterations)
            {
                NextStep();

                SaveCurrentBMP();
            }
            else
            {
                automaticTimer.Stop();
                isAutomaticRunning = false;
                automaticButton.Text = "Автоматично";
                pauseButton.Text = "Пауза";
            }
        }

        private void CreateRules()
        {
            rules.Clear();

            rules.Add(new TurmiteRule('A', 0, 1, -1, 'A'));
            rules.Add(new TurmiteRule('A', 1, 2, -1, 'A'));
            rules.Add(new TurmiteRule('A', 2, 3, -1, 'A'));
            rules.Add(new TurmiteRule('A', 3, 4, -1, 'A'));
            rules.Add(new TurmiteRule('A', 4, 5, -1, 'A'));
            rules.Add(new TurmiteRule('A', 5, 6, 1, 'B'));

            rules.Add(new TurmiteRule('B', 0, 1, 1, 'A'));
            rules.Add(new TurmiteRule('B', 5, 6, -1, 'B'));
            rules.Add(new TurmiteRule('B', 6, 7, -1, 'B'));
            rules.Add(new TurmiteRule('B', 7, 8, -1, 'B'));
            rules.Add(new TurmiteRule('B', 8, 9, -1, 'B'));
            rules.Add(new TurmiteRule('B', 9, 10, -1, 'B'));
            rules.Add(new TurmiteRule('B', 10, 11, -1, 'B'));
            rules.Add(new TurmiteRule('B', 11, 12, -1, 'B'));
            rules.Add(new TurmiteRule('B', 12, 13, -1, 'B'));
            rules.Add(new TurmiteRule('B', 13, 14, -1, 'B'));
            rules.Add(new TurmiteRule('B', 14, 15, -1, 'B'));
            rules.Add(new TurmiteRule('B', 15, 0, -1, 'B'));
        }

        private void ChangeDirection(int turn)
        {
            string[] directions =
            {
                "Північ",
                "Схід",
                "Південь",
                "Захід"
            };

            int currentIndex = Array.IndexOf(directions, turmiteDirection);

            if (turn == -1)
            {
                currentIndex--;
            }
            else if (turn == 1)
            {
                currentIndex++;
            }

            if (currentIndex < 0)
                currentIndex = 3;

            if (currentIndex > 3)
                currentIndex = 0;

            turmiteDirection = directions[currentIndex];
        }

        private void NextStep()
        {
            int currentColor = field[turmitX, turmitY];

            TurmiteRule rule = rules.Find(r =>
                r.CurrentState == turmiteState &&
                r.OldColor == currentColor);

            if (rule == null)
            {
                MessageBox.Show(
                    $"Правило для стану {turmiteState} та кольору {currentColor} не знайдено.",
                    "Помилка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            // Змінюємо колір поточної клітини
            field[turmitX, turmitY] = rule.NewColor;

            // Змінюємо напрямок
            ChangeDirection(rule.Turn);

            // Змінюємо стан
            turmiteState = rule.NewState;

            // Переміщуємо тьюрміта
            MoveTurmite();

            // Збільшуємо номер ітерації
            iterationCount++;

            UpdateInformation();
            fieldPanel.Invalidate();
        }

        private void MoveTurmite()
        {
            switch (turmiteDirection)
            {
                case "Північ":
                    turmitY--;
                    break;

                case "Схід":
                    turmitX++;
                    break;

                case "Південь":
                    turmitY++;
                    break;

                case "Захід":
                    turmitX--;
                    break;
            }

            // Перехід через межі поля
            if (turmitX < 0)
                turmitX = FieldWidth - 1;

            if (turmitX >= FieldWidth)
                turmitX = 0;

            if (turmitY < 0)
                turmitY = FieldHeight - 1;

            if (turmitY >= FieldHeight)
                turmitY = 0;
        }      
        private void CreateInterface()
        {
            AutoScaleMode = AutoScaleMode.None;
            Text = "Клітинний автомат - Тьюрміт";
            ClientSize = new Size(1100, 850);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.WhiteSmoke;
            
            fieldPanel = new Panel();
            fieldPanel.Name = "fieldPanel";
            fieldPanel.Location = new Point(30, 70);
            fieldPanel.Size = new Size(FieldWidth * CellSize, FieldHeight * CellSize);
            fieldPanel.BackColor = Color.Black;
            fieldPanel.Paint += DrawField;
            Controls.Add(fieldPanel);

            Label fieldSizeLabel = new Label();
            fieldSizeLabel.Text = $"Розмір поля: {FieldWidth} x {FieldHeight}";
            fieldSizeLabel.Font = new Font("Calibri", 12);
            fieldSizeLabel.AutoSize = true;
            fieldSizeLabel.Location = new Point(30, 680);
            Controls.Add(fieldSizeLabel);

            Label initialStateTitle = new Label();
            initialStateTitle.Text = "Початковий стан: ";
            initialStateTitle.Font = new Font("Calibri", 12, FontStyle.Bold);
            initialStateTitle.Location = new Point(30, 715);
            initialStateTitle.AutoSize = true;

            Controls.Add(initialStateTitle);

            Label titleLabel = new Label();
            titleLabel.Text = "Тьюрміт";
            titleLabel.Font = new Font("Calibri", 20, FontStyle.Bold);
            titleLabel.AutoSize = true;
            titleLabel.Location = new Point(253, 20);
            Controls.Add(titleLabel);

            Label parametersTitle = new Label();
            parametersTitle.Text = "Параметри:";
            parametersTitle.Font = new Font("Calibri", 14, FontStyle.Bold);
            parametersTitle.AutoSize = true;
            parametersTitle.Location = new Point(660, 80);
            Controls.Add(parametersTitle);

            iterationLabel = new Label();
            iterationLabel.Font = new Font("Calibri", 12);
            iterationLabel.AutoSize = true;
            iterationLabel.Location = new Point(660, 120);
            Controls.Add(iterationLabel);

            stateLabel = new Label();
            stateLabel.Font = new Font("Calibri", 12);
            stateLabel.AutoSize = true;
            stateLabel.Location = new Point(660, 150);
            Controls.Add(stateLabel);

            directionLabel = new Label();
            directionLabel.Font = new Font("Calibri", 12);
            directionLabel.AutoSize = true;
            directionLabel.Location = new Point(660, 180);
            Controls.Add(directionLabel);

            positionLabel = new Label();
            positionLabel.Font = new Font("Calibri", 12);
            positionLabel.AutoSize = true;
            positionLabel.Location = new Point(660, 210);
            Controls.Add(positionLabel);

            Label controlTitle = new Label();
            controlTitle.Text = "Управління:";
            controlTitle.Font = new Font("Calibri", 14, FontStyle.Bold);
            controlTitle.AutoSize = true;
            controlTitle.Location = new Point(660, 245);
            Controls.Add(controlTitle);

            nextStepButton = new Button();
            nextStepButton.Text = "Наступний крок";
            nextStepButton.Size = new Size(280, 45);
            nextStepButton.Location = new Point(660, 290);
            Controls.Add(nextStepButton);
            nextStepButton.Click += (sender, e) => NextStep();

            automaticButton = new Button();
            automaticButton.Text = "Автоматично";
            automaticButton.Size = new Size(280, 45);
            automaticButton.Location = new Point(660, 345);
            Controls.Add(automaticButton);
            automaticButton.Click += AutomaticButton_Click;

            pauseButton = new Button();
            pauseButton.Text = "Пауза";
            pauseButton.Size = new Size(135, 40);
            pauseButton.Location = new Point(805, 400);
            Controls.Add(pauseButton);
            pauseButton.Click += PauseButton_Click;

            resetButton = new Button();
            resetButton.Text = "Скинути";
            resetButton.Size = new Size(135, 40);
            resetButton.Location = new Point(660, 400);
            Controls.Add(resetButton);
            resetButton.Click += ResetButton_Click;

            saveButton = new Button();
            saveButton.Text = "Зберегти bmp";
            saveButton.Size = new Size(280, 45);
            saveButton.Location = new Point(660, 520);
            Controls.Add(saveButton);
            saveButton.Click += SaveButton_Click;

            Label iterationsLabel = new Label();
            iterationsLabel.Text = "Кількість ітерацій:";
            iterationsLabel.Font = new Font("Calibri", 12);
            iterationsLabel.AutoSize = true;
            iterationsLabel.Location = new Point(660, 450);
            Controls.Add(iterationsLabel);

            iterationsTextBox = new TextBox();
            iterationsTextBox.Text = "100";
            iterationsTextBox.Font = new Font("Calibri", 12);
            iterationsTextBox.Size = new Size(100, 30);
            iterationsTextBox.Location = new Point(660, 475);
            Controls.Add(iterationsTextBox);

            executeButton = new Button();
            executeButton.Text = "Виконати";
            executeButton.Font = new Font("Calibri", 12);
            executeButton.Size = new Size(165, 30);
            executeButton.Location = new Point(775, 475);
            Controls.Add(executeButton);
            executeButton.Click += ExecuteButton_Click;

            Label xLabel = new Label();
            xLabel.Text = "X:";
            xLabel.Font = new Font("Calibri", 12);
            xLabel.AutoSize = true;
            xLabel.Location = new Point(200, 715);
            Controls.Add(xLabel);

            xTextBox = new TextBox();
            xTextBox.Text = "100";
            xTextBox.Size = new Size(55, 25);
            xTextBox.Location = new Point(225, 713);
            Controls.Add(xTextBox);

            Label yLabel = new Label();
            yLabel.Text = "Y:";
            yLabel.Font = new Font("Calibri", 12);
            yLabel.AutoSize = true;
            yLabel.Location = new Point(300, 715);
            Controls.Add(yLabel);

            yTextBox = new TextBox();
            yTextBox.Text = "100";
            yTextBox.Size = new Size(55, 25);
            yTextBox.Location = new Point(325, 713);
            Controls.Add(yTextBox);

            Label stateInputLabel = new Label();
            stateInputLabel.Text = "Стан:";
            stateInputLabel.Font = new Font("Calibri", 12);
            stateInputLabel.AutoSize = true;
            stateInputLabel.Location = new Point(400, 715);
            Controls.Add(stateInputLabel);

            stateComboBox = new ComboBox();
            stateComboBox.Items.Add("A");
            stateComboBox.Items.Add("B");
            stateComboBox.SelectedIndex = 0;
            stateComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            stateComboBox.Size = new Size(55, 25);
            stateComboBox.Location = new Point(457, 713);
            Controls.Add(stateComboBox);

            Label directionInputLabel = new Label();
            directionInputLabel.Text = "Напрямок:";
            directionInputLabel.Font = new Font("Calibri", 12);
            directionInputLabel.AutoSize = true;
            directionInputLabel.Location = new Point(527, 715);
            Controls.Add(directionInputLabel);

            directionComboBox = new ComboBox();
            directionComboBox.Items.Add("Схід");
            directionComboBox.Items.Add("Північ");
            directionComboBox.Items.Add("Південь");
            directionComboBox.Items.Add("Захід");
            directionComboBox.SelectedIndex = 0;
            directionComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            directionComboBox.Size = new Size(100, 25);
            directionComboBox.Location = new Point(631, 713);
            Controls.Add(directionComboBox);

            applyButton = new Button();
            applyButton.Text = "Застосувати";
            applyButton.Font = new Font("Calibri", 12);
            applyButton.Size = new Size(140, 30);
            applyButton.Location = new Point(758, 712);
            Controls.Add(applyButton);
            applyButton.Click += ApplyButton_Click;

            rulesButton = new Button();
            rulesButton.Text = "Переглянути правила";
            rulesButton.Font = new Font("Calibri", 12);
            rulesButton.Size = new Size(210, 30);
            rulesButton.Location = new Point(850, 755);
            Controls.Add(rulesButton);
            rulesButton.Click += RulesButton_Click; 

        }

        private void AutomaticButton_Click(object sender, EventArgs e)
        {
            if (isAutomaticRunning)
                return;

            if (!int.TryParse(iterationsTextBox.Text, out automaticTargetIterations))
            {
                MessageBox.Show(
                    "Введіть коректну кількість ітерацій.",
                    "Помилка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            if (automaticTargetIterations <= 0)
            {
                MessageBox.Show(
                    "Кількість ітерацій повинна бути більшою за 0.",
                    "Помилка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            if (iterationCount >= automaticTargetIterations)
            {
                MessageBox.Show(
                    "Вже виконано задану кількість ітерацій.",
                    "Інформація",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                return;
            }

            isAutomaticRunning = true;
            automaticButton.Text = "Виконується...";
            pauseButton.Text = "Пауза";

            automaticTimer.Interval = automaticDelay;
            automaticTimer.Start();
        }

        private void RulesButton_Click(object sender, EventArgs e)
        {
            Form rulesForm = new Form();

            rulesForm.Text = "Правила тьюрміта";
            rulesForm.Size = new Size(500, 600);
            rulesForm.StartPosition = FormStartPosition.CenterParent;

            TextBox rulesTextBox = new TextBox();

            rulesTextBox.Multiline = true;
            rulesTextBox.ReadOnly = true;
            rulesTextBox.ScrollBars = ScrollBars.Vertical;
            rulesTextBox.Font = new Font("Calibri", 12);
            rulesTextBox.Dock = DockStyle.Fill;

            rulesTextBox.Text =
                "Правила тьюрміта:\r\n\r\n" +

                "A, 0 → 1, поворот -1, A\r\n" +
                "A, 1 → 2, поворот -1, A\r\n" +
                "A, 2 → 3, поворот -1, A\r\n" +
                "A, 3 → 4, поворот -1, A\r\n" +
                "A, 4 → 5, поворот -1, A\r\n" +
                "A, 5 → 6, поворот +1, B\r\n\r\n" +

                "B, 0 → 1, поворот +1, A\r\n" +
                "B, 5 → 6, поворот -1, B\r\n" +
                "B, 6 → 7, поворот -1, B\r\n" +
                "B, 7 → 8, поворот -1, B\r\n" +
                "B, 8 → 9, поворот -1, B\r\n" +
                "B, 9 → 10, поворот -1, B\r\n" +
                "B, 10 → 11, поворот -1, B\r\n" +
                "B, 11 → 12, поворот -1, B\r\n" +
                "B, 12 → 13, поворот -1, B\r\n" +
                "B, 13 → 14, поворот -1, B\r\n" +
                "B, 14 → 15, поворот -1, B\r\n" +
                "B, 15 → 0, поворот -1, B";

            rulesForm.Controls.Add(rulesTextBox);

            rulesForm.ShowDialog();
        }


        private void ExecuteButton_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(iterationsTextBox.Text, out int targetIterations))
            {
                MessageBox.Show(
                    "Введіть коректну кількість ітерацій.",
                    "Помилка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            if (targetIterations <= 0)
            {
                MessageBox.Show(
                    "Кількість ітерацій повинна бути більшою за 0.",
                    "Помилка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            automaticTimer.Stop();
            isAutomaticRunning = false;

            automaticButton.Text = "Автоматично";
            pauseButton.Text = "Пауза";

            while (iterationCount < targetIterations)
            {
                NextStep();
            }

            fieldPanel.Invalidate();
        }

        private void ApplyButton_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(xTextBox.Text, out int newX) ||
                !int.TryParse(yTextBox.Text, out int newY))
            {
                MessageBox.Show(
                    "Введіть коректні координати X та Y.",
                    "Помилка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            if (newX < 0 || newX >= FieldWidth ||
                newY < 0 || newY >= FieldHeight)
            {
                MessageBox.Show(
                    $"Координати повинні бути від 0 до {FieldWidth - 1}.",
                    "Помилка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            automaticTimer.Stop();
            isAutomaticRunning = false;

            automaticButton.Text = "Автоматично";
            pauseButton.Text = "Пауза";

            // Очищуємо поле
            for (int y = 0; y < FieldHeight; y++)
            {
                for (int x = 0; x < FieldWidth; x++)
                {
                    field[x, y] = 0;
                }
            }

            iterationCount = 0;

            // Встановлюємо нову позицію
            turmitX = newX;
            turmitY = newY;

            fieldPanel.Invalidate();
        }

        private void ResetButton_Click(object sender, EventArgs e)
        {
            automaticTimer.Stop();
            isAutomaticRunning = false;

            automaticButton.Text = "Автоматично";
            pauseButton.Text = "Пауза";

            ResetAutomaton();
        }

        private void PauseButton_Click(object sender, EventArgs e)
        {
            if (!isAutomaticRunning)
                return;

            if (automaticTimer.Enabled)
            {
                automaticTimer.Stop();
                pauseButton.Text = "Продовжити";
            }
            else
            {
                automaticTimer.Start();
                pauseButton.Text = "Пауза";
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            SaveCurrentBMP();

            MessageBox.Show(
                $"BMP збережено для ітерації {iterationCount}.",
                "Збереження",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void SaveCurrentBMP()
        {
            using (Bitmap bitmap = new Bitmap(
                fieldPanel.Width,
                fieldPanel.Height))
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    for (int y = 0; y < FieldHeight; y++)
                    {
                        for (int x = 0; x < FieldWidth; x++)
                        {
                            using (Brush brush =
                                new SolidBrush(colors[field[x, y]]))
                            {
                                g.FillRectangle(
                                    brush,
                                    x * CellSize,
                                    y * CellSize,
                                    CellSize,
                                    CellSize);
                            }
                        }
                    }

                    // Тьюрміт
                    using (Brush brush = new SolidBrush(Color.Red))
                    {
                        g.FillRectangle(
                            brush,
                            turmitX * CellSize,
                            turmitY * CellSize,
                            CellSize,
                            CellSize);
                    }
                }

                string fileName = $"turmite_{iterationCount:D3}.bmp";

                bitmap.Save(
                    fileName,
                    System.Drawing.Imaging.ImageFormat.Bmp);
            }
        }

        private void ResetAutomaton()
        {
            field = new int[FieldWidth, FieldHeight];

            turmitX = FieldWidth / 2;
            turmitY = FieldHeight / 2;

            turmiteState = 'A';
            turmiteDirection = "Схід";
            iterationCount = 0;

            UpdateInformation();
            fieldPanel.Invalidate();
        }

        private void UpdateInformation()
        {
            iterationLabel.Text = $"Ітерація: {iterationCount}";
            stateLabel.Text = $"Стан: {turmiteState}";
            directionLabel.Text = $"Напрямок: {turmiteDirection}";
            positionLabel.Text = $"Позиція: ({turmitX}, {turmitY})";
        }
        
        private void DrawField(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            for (int y = 0; y < FieldHeight; y++)
            {
                for (int x = 0; x < FieldWidth; x++)
                {
                    Color cellColor = colors[field[x, y]];

                    using (Brush brush = new SolidBrush(cellColor))
                    {
                        g.FillRectangle(
                            brush,
                            x * CellSize,
                            y * CellSize,
                            CellSize,
                            CellSize);
                    }
                }
            }

            // Відображення тьюрміта
            using (Brush brush = new SolidBrush(Color.Red))
            {
                g.FillRectangle(
                    brush,
                    turmitX * CellSize,
                    turmitY * CellSize,
                    CellSize,
                    CellSize);
            }
        }   
    }
}