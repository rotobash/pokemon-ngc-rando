using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using XDCommon.PokemonDefinitions;
using XDCommon.PokemonDefinitions.XD.SaveData;

namespace Randomizer.AP
{
    public partial class PokemonDisplay : UserControl
    {
        public PokemonDisplay()
        {
            InitializeComponent();
            UpdatePokemon(null, null, null);
        }

        private void SetControlVisibility(bool visible)
        {
            foreach (Control control in tableLayoutPanel1.Controls)
            {
                control.Visible = visible;
            }
        }

        public void UpdatePokemon(PokemonInstance pokemon, Pokemon pokemonReference, Move[] moveReference)
        {
            SetControlVisibility(pokemon?.IsSet ?? false);

            if (pokemon?.IsSet == true && pokemonReference != null && moveReference != null && pokemon.CurrentLevel > 0)
            {
                pokemonNameLabel.Text = pokemon.Name.ToString();
                speciesLabel.Text = pokemonReference.Name;
                isShadowLabel.Text = "Unknown";
                abilityLabel.Text = "Unknown";
                levelLabel.Text = $"Level {pokemon.CurrentLevel}";

                progressBar1.Maximum = pokemon.HP;
                progressBar1.Value = pokemon.CurrentHP;

                move1Label.Text = $"{moveReference[pokemon.LearnedMoves[0].Index].Name} {pokemon.LearnedMoves[0].PP}/{moveReference[pokemon.LearnedMoves[0].Index].PP}";
                move2Label.Text = $"{moveReference[pokemon.LearnedMoves[1].Index].Name} {pokemon.LearnedMoves[1].PP}/{moveReference[pokemon.LearnedMoves[1].Index].PP}";
                move3Label.Text = $"{moveReference[pokemon.LearnedMoves[2].Index].Name} {pokemon.LearnedMoves[2].PP}/{moveReference[pokemon.LearnedMoves[2].Index].PP}";
                move4Label.Text = $"{moveReference[pokemon.LearnedMoves[3].Index].Name} {pokemon.LearnedMoves[3].PP}/{moveReference[pokemon.LearnedMoves[3].Index].PP}";
            }
            else
            {
                pokemonNameLabel.Visible = true;
                pokemonNameLabel.Text = "Not set";
            }
        }
    }
}
