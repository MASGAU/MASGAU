<?php

class SpecialGameCompatibility extends SpecialPage {

    function __construct() {
        parent::__construct('GameCompatibility');
    }

    private static $database = 'masgau_game_data';
    private static $criterias;

    function execute($par) {
        global $wgRequest, $wgOut;


        $this->setHeaders();

        require_once 'GamesList.php';
        self::$criterias = getGameLetters(true);

        $par = $wgRequest->getText('title');
        $bits = explode('/', trim($par));
        
        $criteria_index = 'A';
        $true_index = 'A';
        $table = 'current';
        $compatGroups = array();
        # Get request data from, e.g.
        $request_string = "";
        $compatGroups['disc'] = 1;
        $compatGroups['gog'] = 1;
        $compatGroups['ps'] = 1;
        $compatGroups['other'] = 1;
        $compatGroups['steam'] = 1;
        foreach($bits as $bit) {
            switch($bit) {
                case "current":
                case "upcoming":
                    $table = $bit;
                    $request_string .= $bit."/";
                    break;
                default:
                    if(substr($bit,0,3)=="no_") {
                        $compatGroups[substr($bit,3,strlen($bit)-3)] = 0;
                        $request_string .= $bit."/";
                    } else {
                        if (array_key_exists($bit, self::$criterias)) {
                            $criteria_index = $bit;
                            $true_index = $bit;
                        } else if($bit=="NUMBERS") {
                            $criteria_index = '#';
                            $true_index = $bit;
                        }
                    }
                    break;
            }
        }

        if($table=="upcoming") {
                $wgOut->setPagetitle("Upcoming Game Compatibility");
        } else {
                $wgOut->setPagetitle("Current Game Compatibility - " . $criteria_index);
        }

        $wgOut->addHTML($param);

        # Do stuff
        # ...


        $dbr = wfGetDB(DB_SLAVE);

        $wgOut->addHTML('<h2>Introduction</h2>');
        
        $res = $dbr->select(array('games' => self::$database . '.games', //
            'compat' => self::$database . '.' . $table . '_compatibility'), //
                array('type', 'count(*) as count'), // $vars (columns of the table)
                'games.name = compat.game', // $conds
                __METHOD__, // $fname = 'Database::select',
                array('GROUP BY' => 'type')
        );
        
        $counts = array();
        $i = 0;
        $count = 0;
        $game_counts = array();
        foreach($res as $row) {
            $counts[$row->type] = $row->count;
            $game_counts[$i] = $row->count;
            if($row->count==1)
                $game_counts[$i] .= ' '.$row->type;
            else 
                $game_counts[$i] .= ' '.$row->type.'s';
            $count += $row->count;
            $i++;
        }
        $count_string = '';
        if($count>0) {
            for($i = 0;$i<sizeof($game_counts);$i++) {
                $count_string .= $game_counts[$i];
                if($i<sizeof($game_counts)-2) {
                    $count_string .= ', ';
                } else if($i<sizeof($game_counts)-1) {
                    $count_string .= ' and ';
                }
            }
            if(sizeof($game_counts)>1) {
                $count_string .= ' ('.$count.' total)';
            }
        } else {
            $count_string = "currently no games (for now)";
        }
        
        if ($table == "current") {
            $res = $dbr->select(array('games' => 'masgau_game_data.xml_files'), //
                    'max(last_updated) as date', // $vars (columns of the table)
                    null, // $conds
                    __METHOD__, // $fname = 'Database::select',
                    null
            );
            $wgOut->addHTML('<p>This list reflects the game compatibility of the current data, which was released on ' . $res->fetchObject()->date . '.</p>');
            $wgOut->addHTML('<p>According to this list '.$count_string.' are currently supported across various platforms.</p>');
        } else {
            $wgOut->addHTML('<p>This list reflects the changes in game compatibility of the upcoming data release.</p>');
            $wgOut->addHTML('<p>According to this list '.$count_string.' will be added/updated with the next data update.</p>');
        }

        $wgOut->addHTML('<p>Unless implicitly stated only the US English install locations are supported. If you know the paths for other languages, please let me know.');
        $wgOut->addHTML('<p>If your experience differs from what is presented here (including a Yes instead of an Untested), please e-mail me at sanmadjack@users.sourceforge.net. If a game that says Yes is not detected on your computer or if you have the path for a No, please send me the path to your save games, or at least the install path. We will figure it out.');


        $wgOut->addHTML('<h2>Color Key</h2>');
        $wgOut->addHTML('<table class="wikitable compatibility" cellpadding="5" cellspacing="0" border="1"><tr class="compatibility">');

        require_once('CompatabilityTable.php');


        // Loads up the game status values
        global $status_res;
        $status_res->seek(0);
        foreach ($status_res as $status) {
            $wgOut->addHTML('<td class="compatibility-' . $status->state . '">' . $status->title . ' - ' . $status->description . '</td>');
        }

        $wgOut->addHTML('</tr></table>');

        $wgOut->addHTML('<h2>Games</h2>');
        if ($table == "current") {
            $wgOut->addHTML('<div style="width:100%;text-align:center;">');
            $links = '';
            foreach (array_keys(self::$criterias) as $key) {
                if ($key == $criteria_index) {
                    $links .= '\'\'\'' . $key . '\'\'\'';
                } else {
                    if ($key == '#')
                        $links .= '[[Special:GameCompatibility/' . $request_string . 'NUMBERS|' . $key . ']]';
                    else
                        $links .= '[[Special:GameCompatibility/' . $request_string . urlencode($key) . '|' . $key . ']]';
                }
                $links .= ' ';
            }
            $wgOut->addWikiText($links);

            $wgOut->addHTMl('</div>');
            $selected_criteria = array(self::$criterias[$criteria_index],'games.name = compat.game');
        }
        else {
            $selected_criteria = array('games.name = compat.game');
        }

        $res = $dbr->select(array('games' => self::$database . '.games', //
            'compat' => self::$database . '.' . $table . '_compatibility'), //
                array('*'), // $vars (columns of the table)
                $selected_criteria, // $conds
                __METHOD__, // $fname = 'Database::select',
                array('ORDER BY' => 'name ASC')
        );


        //$wgOut->addHTML('<caption>'.$criteria_index.'</caption>');


        if ($res->numRows() > 0) {
            require_once('CompatabilityTable.php');
            beginCompatTable();
            drawCompatRows($res,$compatGroups);
            endCompatTable();
        } else {
            if($letter==null) {
                $wgOut->addHTML('<h3 style="width:100%;text-align:center;">There are currently no games in this list</h3>');
            } else {
                $wgOut->addHTML('<h3 style="width:100%;text-align:center;">There are currently no supported games for the letter ' . $letter . '</h3>');
            }
        }
    }

}