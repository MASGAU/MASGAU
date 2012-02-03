<?php

function getGameLetters($only_compatible) {
    $dbr = wfGetDB(DB_SLAVE);
    if($only_compatible) {
    $res = $dbr->select(array('games' => 'masgau_game_data.games',
        'compat'=>'masgau_game_data.current_compatibility'), //
            array('substr(name,1,1) as letter'), // $vars (columns of the table)
            'games.name = compat.game', // $conds
            __METHOD__, // $fname = 'Database::select',
            array('GROUP BY' => 'letter',
        'ORDER BY' => 'letter asc')
    );
    } else {
    $res = $dbr->select(array('games' => 'masgau_game_data.games'), //
            array('substr(name,1,1) as letter'), // $vars (columns of the table)
            null, // $conds
            __METHOD__, // $fname = 'Database::select',
            array('GROUP BY' => 'letter',
        'ORDER BY' => 'letter asc')
    );
    }

    $letters = array();
    foreach ($res as $row) {
        if(is_numeric($row->letter)) {
            $letters['#'] = 'name REGEXP \'^[0-9]\'';
        } else {
            $letter = strtoupper($row->letter);
            $letters[$letter] = 'name like "'.$letter.'%"';
        }
    }
    return $letters;
}

?>