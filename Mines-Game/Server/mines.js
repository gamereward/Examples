//test
var game; //Game object
var tableSize = 25; //5x5 table size

/// start a newgame
/// @param bet - bet amount
/// @param mines - number of mines
public function newgame(bet,mines){
  setSessionStore("mine");
  if(getBalance()>=bet){
      newUserSession();
      game={};
      game.error=0;
      game.bet=bet;
      game.mines=mines;
      game.payout=0;
      game.nextProfit=0;
      game.status=0;
      game.minesIndex=[];
      game.playIndex=[];
      game.win=0;
      initTable();
      saveUserSessionData("gamedata",JSON.stringify(game));
      chargeMoney(bet);
      return game;
    }else{
      return {'error':1};
    }
}

/// open a tile
/// @param pos - open position
public function open(pos){
  setSessionStore("mine");
    game=JSON.parse(loadUserSessionData("gamedata"));
    if(game!=null){
      game.playIndex.push(parseInt(pos));
      if(game.minesIndex.indexOf(parseInt(pos))>-1){
        game.status=1;
        game.win=-1;
        game.payout=0;
        saveUserSessionData("gamedata",JSON.stringify(game));
        return game;
      }else if(game.playIndex.length==tableSize-game.minesIndex.length){
        game.status=1;
		game.payout=payoutCal(game.mines,game.playIndex.length)*game.bet;
		game.win=1;
        chargeMoney(-game.payout);
        saveUserSessionData("gamedata",JSON.stringify(game));
        return game;
      }else {
        game.error=0;
        game.status=0;
		game.payout=payoutCal(game.mines,game.playIndex.length)*game.bet;
		game.win=0;
		game.nextProfit = (payoutCal(game.mines,game.playIndex.length+1)
                           -payoutCal(game.mines,game.playIndex.length))*game.bet;
        saveUserSessionData("gamedata",JSON.stringify(game));
		return game;
      }
    }else{
      return {'error':1};
    }
}

/// request payout
public function payout(){
  setSessionStore("mine");
  game=JSON.parse(loadUserSessionData("gamedata"));
  if(game!=null){
    if(game.win==0&&game.status==0){
      game.win=1;
	  game.status=1;
      saveUserSessionData("gamedata",JSON.stringify(game));
      chargeMoney(-game.payout);
      return game;
    }else{
      return {'error':1};
    }
  }else{
    return {'error':1};
  }
  
}

/// specify mines position on table
function initTable() {
    while(game.minesIndex.length < game.mines){
        var index = Math.floor(Math.random() * tableSize);
        if (game.minesIndex.indexOf(index) < 0) game.minesIndex.push(index);
    }
}

/// calculate payout for current game
function payoutCal(mines,diamonds){
        var house_edge = 0.01;
        return (1 - house_edge) * nCr(tableSize,diamonds) / nCr(tableSize - mines, diamonds);
}


function nCr(n,r){
        var result = Factorial(n) / Factorial(r) / Factorial(n - r);
        return result;

}

function Factorial(i)
{
		if (i <= 1)
			return 1;
		return i * Factorial(i - 1);
}
