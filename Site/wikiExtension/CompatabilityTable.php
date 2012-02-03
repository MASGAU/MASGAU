<?php
    global $status_res;
    $status_res = $dbr->select('masgau_game_data.compatibility_states', '*', // $vars (columns of the table)
                    null, // $conds
                    __METHOD__, // $fname = 'Database::select',
                    null
                );
    global $statuses;
    $statuses = array();
    foreach ($status_res as $row) {
        $statuses[$row->state] = $row->title;
    }

    function drawCompatRows($res,$compatGroups) {
        global $wgOut;
        $i = 0;
        $max_games = 50;
        foreach ($res as $row) {
            if ($i == 0) {
                // Prints the table header every 50 or so, or when we're at a new letter
                drawCompatHeader($compatGroups);
            }
            drawCompatRow($row,null,$compatGroups);
            $i++;
            if ($i == 50)
                $i = 0;
        }
    }
    
    function drawCompatRow($row, $name_override, $compatGroups) {
        global $wgOut;
        $wgOut->addHTML('<tr class="compatibility">');
        if($name_override==null) {
            if ($row->game!=null) {
                $wgOut->addHTML('<th>');
                $wgOut->addWikiText('[[Special:GameData/' . $row->game . '|' . $row->title . ']]');
                $wgOut->addHTML('</th>');
            } else {
                $wgOut->addHTML('<th>');
                $wgOut->addWikiText($row->title);
                $wgOut->addHTML('</th>');
            }
        } else {
            $wgOut->addHTML('<th>' . $name_override . '</th>');
        }
        if($compatGroups['disc']!=0)
            outputCompatGroup($row->disc_xp, $row->disc_vista, $row->disc_reg_key, $row->disc_shortcut);
        if($compatGroups['steam']!=0)
            outputCompatGroup($row->steam_xp, $row->steam_vista, $row->steam_cloud);
        if($compatGroups['gog']!=0)
            outputCompatGroup($row->gog_xp, $row->gog_vista, $row->gog_reg_key, $row->gog_shortcut);
        if($compatGroups['other']!=0)
            outputCompatGroup($row->other_xp, $row->other_vista, $row->other_reg_key, $row->other_shortcut);
        if($compatGroups['ps']!=0)
            outputCompatGroup($row->ps1_id, $row->ps2_id, $row->ps3_id, $row->psp_id);
        $wgOut->addHTML('<td>' . $row->comment . '</td>');
        $wgOut->addHTML('</tr>');
    }


    function beginCompatTable() {
        global $wgOut;
        $wgOut->addHTML('<table class="wikitable compatibility" cellpadding="5" cellspacing="0" border="1">');
    }
    
    function endCompatTable() {
        global $wgOut;
        $wgOut->addHTML('</table>');
    
    }
    
    function drawCompatHeader($compatGroups) {
        global $wgOut;
        $wgOut->addHTML('<tr class="compatibility_header">');
        $wgOut->addHTML('<th rowspan="2"></th>');
        if($compatGroups['disc']!=0)
            $wgOut->addHTML('<th colspan="4">Disc</th>');
        if($compatGroups['steam']!=0)
            $wgOut->addHTML('<th colspan="3"><a href="http://store.steampowered.com/">Steam</a></th>');
        if($compatGroups['gog']!=0)
            $wgOut->addHTML('<th colspan="4"><a href="http://www.gog.com/">GoG</a></th>');
        if($compatGroups['other']!=0)
            $wgOut->addHTML('<th colspan="4">Other Download</th>');
        if($compatGroups['ps']!=0)
            $wgOut->addHTML('<th colspan="4">Playstation IDs</th>');
        $wgOut->addHTML('<th rowspan="2" style="width:auto;">Note(s)</th>');
        $wgOut->addHTML('</tr>');
        $wgOut->addHTML('<tr class="compatibility_header">');
        if($compatGroups['disc']!=0)
            $wgOut->addHTML('<th>XP</th><th>Vista/7</th><th>Reg. Key</th><th>Shortcut</th>');
        if($compatGroups['steam']!=0)
            $wgOut->addHTML('<th>XP</th><th>Vista/7</th><th>Cloud</th>');
        if($compatGroups['gog']!=0)
            $wgOut->addHTML('<th>XP</th><th>Vista/7</th><th>Reg. Key</th><th>Shortcut</th>');
        if($compatGroups['other']!=0)
            $wgOut->addHTML('<th>XP</th><th>Vista/7</th><th>Reg. Key</th><th>Shortcut</th>');
        if($compatGroups['ps']!=0)
            $wgOut->addHTML('<th>PS1</th><th>PS2</th><th>PS3</th><th>PSP</th>');
        $wgOut->addHTML('</tr>');

    }


    
    function outputCompatGroup() {
        global $wgOut, $statuses;
        $args = func_get_args();
        $last_arg = null;
        $column_count = 1;
        for ($i = 0; $i < sizeof($args); $i++) {
            $column_count = 1;
            while ($args[$i + $column_count] == $args[$i]) {
                $column_count++;
            }
            $wgOut->addHTML('<td class="compatibility-' . $args[$i] . '" colspan="' . $column_count . '">');
            $wgOut->addHTML($statuses[$args[$i]]);
            $wgOut->addHTML('</td>');


            $i += $column_count - 1;
        }
        return;
    }
?>
