<?php
class SpecialContributor extends SpecialPage {
        function __construct() {
                parent::__construct( 'Contributor' );
        }
 
        function execute( $par ) {
                global $wgRequest, $wgOut;
 
                $this->setHeaders();
 
         $par = $wgRequest->getText('title');
        $bits = explode( '/', trim( $par ) );
        
        # Get request data from, e.g.
        if (sizeof($bits[1])>0) {
            $contributor = $bits[1];
        } else {
            $contributor = 'SanMadJack';
        }

        
        
        $dbr = wfGetDB(DB_SLAVE);
        $res = $dbr->select(array('con'=>'masgau_game_data.contributions',
            'ver' =>'masgau_game_data.game_versions',
            'game' =>'masgau_game_data.games'), array('*'), // $vars (columns of the table)
                        array('contributor = \''    . $contributor . '\'',
                            'con.game_version = ver.id',
                            'game.name = ver.name'), // $conds
                        __METHOD__, // $fname = 'Database::select',
                        array('ORDER BY' => 'game.name ASC')
        );
        
        if ($res->numRows()== 0) {
            $contributor = str_replace('_',' ',$contributor);
            $res = $dbr->select(array('con'=>'masgau_game_data.contributions',
                'ver' =>'masgau_game_data.game_versions',
                'game' =>'masgau_game_data.games'), array('*'), // $vars (columns of the table)
                            array('contributor = \''    . $contributor . '\'',
                                'con.game_version = ver.id',
                                'game.name = ver.name'), // $conds
                            __METHOD__, // $fname = 'Database::select',
                            array('ORDER BY' => 'game.name ASC')
            );
            
        }        
        
        $wgOut->setPagetitle("Contributions By " . $contributor);

        if ($res->numRows()== 0) {
            $wgOut->addWikiText($contributor.' has made '.$res->numRows().' contributions :(');
        } else {
            $wgOut->addWikiText($contributor.' has made '.$res->numRows().' contributions!');
            
            $wgOut->addHTML('<h2>Games Contributed</h2>');
            
            $per_column = ceil((float)$res->numRows() /(float)3);
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


                if($last_letter==null||$last_letter!=$letter) {
                    $wgOut->addHTML('<h3>'.$letter.'</h3>');
                }
                $text = '* [[Special:GameData/'.$row->name.'|'.$row->title.']]';
                if($row->platform!=UNIVERSAL)
                    $text .= ' ('.$row->platform.')';
                
                if($row->region!=UNIVERSAL)
                    $text .= ' ('.$row->region.')';
                
                $wgOut->addWikiText($text);

                $last_letter = $letter;
                    $column_count++;
                if($column_count>=$per_column) {
                    $wgOut->addHTML('</div>');
                    $column_count = null;
                }
            }
            
        }
        
        
 
        }
}