using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MASGAU.Game
{
    class DetectedGames : MVC.FilteredModel<GameID, GameEntry>
    {
        private bool _ShowUndetectedGames = false;
        public bool ShowUndetectedGames
        {
            get
            {
                return _ShowUndetectedGames;
            }
            set
            {
                this.ClearFilters();
                if (value)
                {
                    this.AddFilter("IsDetectedOrHasArchives", true);
                }
                else
                {
                    this.AddFilter("IsDetected", true);
                }
               
                _ShowUndetectedGames = value;
            }
        }

        public DetectedGames(MVC.Model<GameID, GameEntry> model)
            : base(model)
        {
            this.ShowUndetectedGames = Core.settings.ShowUndetectedGames;
        }


    }
}
