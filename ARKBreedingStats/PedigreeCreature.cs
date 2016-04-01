﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ARKBreedingStats
{

    public partial class PedigreeCreature : UserControl
    {
        private Creature creature;
        public delegate void CreatureChangedEventHandler(Creature creature, int comboId);
        public event CreatureChangedEventHandler CreatureClicked;
        private List<Label> labels;
        ToolTip tt = new ToolTip();
        public int comboId;
        public bool onlyLevels; // no gender, status, colors
        public bool[] enabledColorRegions;

        public PedigreeCreature()
        {
            InitC();
        }
        private void InitC()
        {
            InitializeComponent();
            tt.InitialDelay = 100;
            tt.SetToolTip(labelHP, "Health");
            tt.SetToolTip(labelSt, "Stamina");
            tt.SetToolTip(labelOx, "Oxygen");
            tt.SetToolTip(labelFo, "Food");
            tt.SetToolTip(labelWe, "Weight");
            tt.SetToolTip(labelDm, "Melee Damage");
            tt.SetToolTip(labelSp, "Speed");
            tt.SetToolTip(labelGender, "Gender");
            labels = new List<Label> { labelHP, labelSt, labelOx, labelFo, labelWe, labelDm, labelSp };
            this.SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
        }

        public PedigreeCreature(Creature creature, bool[] enabledColorRegions, int comboId = -1)
        {
            InitC();
            this.Cursor = Cursors.Hand;
            this.enabledColorRegions = enabledColorRegions;
            this.comboId = comboId;
            setCreature(creature);
        }
        public void setCreature(Creature creature)
        {
            this.creature = creature;
            groupBox1.Text = (!onlyLevels && creature.status != CreatureStatus.Available ? "(" + Utils.statusSymbol(creature.status) + ") " : "") + creature.name;
            if (!onlyLevels && creature.status == CreatureStatus.Dead)
            {
                groupBox1.ForeColor = SystemColors.GrayText;
                tt.SetToolTip(groupBox1, "Creature has passed away");
            }
            else if (!onlyLevels && creature.status == CreatureStatus.Unavailable)
            {
                groupBox1.ForeColor = SystemColors.GrayText;
                tt.SetToolTip(groupBox1, "Creature is currently not available");
            }

            for (int s = 0; s < 7; s++)
            {
                if (creature.levelsWild[s] < 0)
                {
                    labels[s].Text = "?";
                    labels[s].BackColor = Color.WhiteSmoke;
                    labels[s].ForeColor = Color.LightGray;
                }
                else
                {
                    labels[s].Text = creature.levelsWild[s].ToString();
                    labels[s].BackColor = Utils.getColorFromPercent((int)(creature.levelsWild[s] * 2.5), (creature.topBreedingStats[s] ? 0.2 : 0.7));
                    labels[s].ForeColor = SystemColors.ControlText;
                }
                labels[s].Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, (creature.topBreedingStats[s] ? System.Drawing.FontStyle.Bold : System.Drawing.FontStyle.Regular), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            }
            if (onlyLevels)
            {
                labelGender.Visible = false;
                pictureBox1.Visible = false;
            }
            else
            {
                labelGender.Visible = true;
                labelGender.Text = Utils.genderSymbol(creature.gender);
                labelGender.BackColor = Utils.genderColor(creature.gender);
                // creature Colors
                pictureBox1.Image = CreatureColored.getColoredCreature(creature.colors, "", enabledColorRegions, 24, 22, true);
                labelGender.Visible = true;
                pictureBox1.Visible = true;
            }
        }
        public bool highlight
        {
            set
            {
                panelHighlight.Visible = value;
                HandCursor = !value;
            }
        }

        public bool HandCursor { set { Cursor = (value ? Cursors.Hand : Cursors.Default); } }

        private void PedigreeCreature_Click(object sender, EventArgs e)
        {
            if (CreatureClicked != null)
                CreatureClicked(this.creature, comboId);
        }

        private void element_Click(object sender, EventArgs e)
        {
            PedigreeCreature_Click(sender, e);
        }

        public void Clear()
        {
            for (int s = 0; s < 7; s++)
            {
                labels[s].Text = "";
                labels[s].BackColor = SystemColors.Control;
            }
            labelGender.Visible = false;
            groupBox1.Text = "";
            pictureBox1.Visible = false;
        }
    }
}
