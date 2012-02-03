<?php

class SpecialGames extends SpecialPage {

    function __construct() {
        parent::__construct('Games');
    }

    function execute($par) {
        global $wgRequest, $wgOut;

        $this->setHeaders();
        $wgOut->setPagetitle("Games");
        # Get request data from, e.g.
        $param = $wgRequest->getText('param');

        require_once 'GamesList.php';
        $criterias = getGameLetters(false);
        
        $wgOut->addHTML('<div style="width:100%;text-align:center;">');
        $links = '';
        foreach (array_keys($criterias) as $key) {
            if ($key == $criteria_index) {
                $links .= '\'\'\''. $key . '\'\'\'';
            } else {
                if($key=='#')
                    $links .= '[[Special:Games#NUMBERS|' . $key . ']]';
                else
                    $links .= '[[Special:Games#'. urlencode($key) . '|' . $key . ']]';
            }
            $links .= ' ';
        }
        $wgOut->addWikiText($links);
        $wgOut->addHTMl('</div>');
        
        
        $dbr = wfGetDB(DB_SLAVE);
        $res = $dbr->select(array('games' => 'masgau_game_data.games'
                ), //
                'count(*) as count', // $vars (columns of the table)
                null, // $conds
                __METHOD__, // $fname = 'Database::select',
                null
        );

        $count = $res->fetchObject()->count;
        $per_column = ceil((float)$count / (float)3);

        $res = $dbr->select(array('games' => 'masgau_game_data.games'
                ), //
                '*', // $vars (columns of the table)
                null, // $conds
                __METHOD__, // $fname = 'Database::select',
                'order by name ASC'
        );
        
        $last_letter = null;
        $column_count = null;
        foreach($res as $row) {
            $letter = strtoupper(substr($row->name,0,1));
            if(is_numeric($letter))
                $letter = '#';
            
            if($column_count==null) {
                $wgOut->addHTML('<div style="width:30%;float:left;overflow:hidden;">');
                if($last_letter!=null&&$last_letter==$letter) {
                    $wgOut->addHTML('<h3>'.$letter.' Cont.</h3>');
                }
                $column_count = 0;
            }
            
            
            if($last_letter==null||$last_letter!=$letter)  {
                if($letter=='#') 
                    $wgOut->addHTML('<h3><a style="color:black;" name="NUMBER">#</a></h3>');
                else
                    $wgOut->addHTML('<h3><a style="color:black;" name="'.$letter.'">'.$letter.'</a></h3>');
            }
            
            $wgOut->addWikiText('* [[Special:GameData/'.$row->name.'|'.$row->title.']]');
            
            $last_letter = $letter;
                $column_count++;
            if($column_count>=$per_column) {
                $wgOut->addHTML('</div>');
                $column_count = null;
            }
        }
        
    }

}