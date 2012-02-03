<?php

class SpecialContributors extends SpecialPage {

    function __construct() {
        parent::__construct('Contributors');
    }

    function execute($par) {
        global $wgRequest, $wgOut;

        $this->setHeaders();

        $dbr = wfGetDB(DB_SLAVE);
        $res = $dbr->select(array('con' => 'masgau_game_data.contributions'), array('contributor', 'count(*) as count'), // $vars (columns of the table)
                null, // $conds
                __METHOD__, // $fname = 'Database::select',
                array('GROUP BY' => 'contributor', 'ORDER BY' => 'count desc, contributor asc')
        );

        $wgOut->setPagetitle("Contributors");

        if ($res->numRows() == 0) {
            $wgOut->addWikiText('Where the hell are all the contributors!?!');
        } else {
            $per_column = ceil((float)$res->numRows() / (float)3);
            $column_count = null;
            foreach ($res as $row) {
                if ($column_count == null) {
                    $wgOut->addHTML('<div style="width:30%;float:left;overflow:hidden;">');
                    $column_count = 0;
                }
                $wgOut->addHTML('<li type="1" value="' . $row->count . '">');
                $wgOut->addWikiText('[[Special:Contributor/' . $row->contributor . '|' . $row->contributor . ']]');
                $wgOut->addHTML('</li>');
                
                $column_count++;
                if($column_count>=$per_column) {
                    $wgOut->addHTML('</div>');
                    $column_count = null;
                }
            }
        }
    }

}