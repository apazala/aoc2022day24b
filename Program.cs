internal class Program
{
    class Blizzard{
        public static int lastCol;
        public static int lastRow;

        int iMove;
        int jMove;
        public int i;
        public int j;
        public static void Init(int lCol, int lRow)
        {
            lastCol = lCol;
            lastRow = lRow;
        }

        public Blizzard(char dir, int iInit, int jInit)
        {
            i = iInit;
            j = jInit;
            switch(dir)
            {
                case '>':
                    iMove = 0;
                    jMove = 1;
                    break;
                
                case '<':
                    iMove = 0;
                    jMove = -1;
                    break;

                case '^':
                    iMove = -1;
                    jMove = 0;
                    break;
                
                case 'v':
                    iMove = 1;
                    jMove = 0;
                    break;
                
                default:
                    throw new Exception("Unexpected Direction!!");
            }

        }

        public void Move()
        {
            i+=iMove;
            j+=jMove;
            if(i == 0){
                i = lastRow;
            }else if(i > lastRow){
                i = 1;
            }else if(j == 0){
                j = lastCol;
            }else if(j > lastCol){
                j = 1;
            }
        } 


    }

    class ValleyState{

        public static int NCOLS {get; set;}
        public static int NROWS {get; set;}

        const int INVALID = -1;
        const int NONVISITED = 0;
        const int VISITED = 1;
        int[,] ground;

        public ValleyState(List<Blizzard> blizzards)
        {
            ground = new int[Blizzard.lastRow+2,Blizzard.lastCol+2];
            //horz borders
            for(int j = 0; j < NCOLS; j++)
            {
                ground[0, j] = INVALID;
                ground[NROWS-1, j] = INVALID;
            }
            //vert borders
            for(int i = 0; i < NROWS; i++)
            {
                ground[i, 0] = INVALID;
                ground[i, NCOLS-1] = INVALID;
            }
            //start & goal
            ground[0, 1] = NONVISITED;
            ground[NROWS-1, NCOLS-2] = NONVISITED;
            
            //the rest are already marked NONVISITED = 0
            foreach(Blizzard b in blizzards)
                MarkBlizzard(b);

        }

        private void MarkBlizzard(Blizzard blizzard)
        {
            ground[blizzard.i, blizzard.j] = INVALID;
        }

        public bool OccupyIfPossible(int i, int j)
        {
            if(ground[i, j] != NONVISITED) return false;
            ground[i, j] = VISITED;
            return true;
        }
        
    }

    class Player{
        public bool start;
        public bool goal;
        public int i;
        public int j;

        public Player()
        {
            start = true;
            goal = false;
            i = 0;
            j = 1;
        }
        public Player(int iInit, int jInit)
        {
            start = (iInit == 0);
            goal = (iInit == ValleyState.NROWS-1);
            i = iInit;
            j = jInit;
        }


        public bool InGoal()
        {
            return goal;
        }

        public bool InStart()
        {
            return start;
        }
        public Player? CanMove(char dir, ValleyState valley)
        {
            Player? newPosition = null;
            if(start){
                switch(dir){
                    case 'x': 
                        if(valley.OccupyIfPossible(0, 1))
                            newPosition = new Player();
                            break;
                    
                    case 'v':
                        if(valley.OccupyIfPossible(1, 1))
                            newPosition = new Player(1, 1);
                            break;

                }

                return newPosition;
            }

            if(goal){
                switch(dir){
                    case 'x':
                        if(valley.OccupyIfPossible(ValleyState.NROWS-1, ValleyState.NCOLS-2))
                            newPosition = new Player(ValleyState.NROWS-1, ValleyState.NCOLS-2);
                        break;
                    case '^':
                        if(valley.OccupyIfPossible(ValleyState.NROWS-2, ValleyState.NCOLS-2))
                            newPosition = new Player(ValleyState.NROWS-2, ValleyState.NCOLS-2);
                            break;
                }
                return newPosition;
            }

            int iMove, jMove;
            switch(dir)
            {
                case 'x':
                    iMove = 0;
                    jMove = 0;
                    break;

                case '>':
                    iMove = 0;
                    jMove = 1;
                    break;
                
                case '<':
                    iMove = 0;
                    jMove = -1;
                    break;

                case '^':
                    iMove = -1;
                    jMove = 0;
                    break;
                
                case 'v':
                    iMove = 1;
                    jMove = 0;
                    break;
                
                default:
                    throw new Exception("Unexpected Direction!!");
            }

            int iNew = this.i+iMove; 
            int jNew = this.j + jMove;
            if(valley.OccupyIfPossible(iNew, jNew))
                newPosition = new Player(iNew, jNew);
            
            return newPosition;
        }

    }

    private static int GCD(int a, int b)
    {
        int c;
        if(a < b)
        {
            c = a; a = b; b = c;
        }

        while(b > 0){
            c = a%b;
            a = b;
            b = c;
        }

        return a;

    }

    private static ValleyState[] generateAllValleyStates(string[] valleyGround)
    {
        int nrows = valleyGround.Length - 2;
        int ncols = valleyGround[0].Length - 2;
        int nstates = ncols*nrows/GCD(ncols, nrows);

        ValleyState[] valleyStates = new ValleyState[nstates];

        Blizzard.Init(ncols, nrows);
        ValleyState.NCOLS = ncols+2;
        ValleyState.NROWS = nrows+2;

        List<Blizzard> blizzardsList = new List<Blizzard>();

        for(int i = 1; i <= nrows; i++)
        {
            for(int j = 1; j <= ncols; j++)
            {
                char dir = valleyGround[i][j];
                if(dir != '.')
                {
                    blizzardsList.Add(new Blizzard(dir, i, j));
                }
            }
        }

        for(int k = 0; k < nstates; k++){
            valleyStates[k] = new ValleyState(blizzardsList);
            foreach(Blizzard b in blizzardsList)
                b.Move();
        }

        return valleyStates;
    }


    private static void Main(string[] args)
    {
        
        string[] valleyGround = File.ReadAllLines("input.txt");

        //First trip
        ValleyState[] valleyStates =  generateAllValleyStates(valleyGround);
        valleyStates[0].OccupyIfPossible(0, 1);
        Queue<Player> queue = new Queue<Player>();
        queue.Enqueue(new Player());

        int count, stateIndex = 0;
        bool goalReached = false;
        for(count = 0; queue.Count > 0 && !goalReached; count++)
        {
            stateIndex = (stateIndex == valleyStates.Length-1?0:stateIndex+1);
            ValleyState currentState = valleyStates[stateIndex];

            int qSize = queue.Count;
            Player? newPosition = null, currentPlayer;
            for(int k = 0; k < qSize; k++)
            {
                currentPlayer = queue.Dequeue();
                if(currentPlayer.InGoal()){
                    goalReached = true;
                    break;
                }

                newPosition = currentPlayer.CanMove('x', currentState);
                if(newPosition != null)
                    queue.Enqueue(newPosition);

                newPosition = currentPlayer.CanMove('<', currentState);
                if(newPosition != null)
                    queue.Enqueue(newPosition);

                newPosition = currentPlayer.CanMove('>', currentState);
                if(newPosition != null)
                    queue.Enqueue(newPosition);

                newPosition = currentPlayer.CanMove('^', currentState);
                if(newPosition != null)
                    queue.Enqueue(newPosition);

                newPosition = currentPlayer.CanMove('v', currentState);
                if(newPosition != null)
                    queue.Enqueue(newPosition);
            }
        }

        //Second trip        
        count--;
        stateIndex--;
        valleyStates =  generateAllValleyStates(valleyGround);
        valleyStates[stateIndex].OccupyIfPossible(ValleyState.NROWS-1, ValleyState.NCOLS-2);
        queue.Clear();
        queue.Enqueue(new Player(ValleyState.NROWS-1, ValleyState.NCOLS-2));


        goalReached = false;
        for(; queue.Count > 0 && !goalReached; count++)
        {
            stateIndex = (stateIndex == valleyStates.Length-1?0:stateIndex+1);
            ValleyState currentState = valleyStates[stateIndex];

            int qSize = queue.Count;
            Player? newPosition = null, currentPlayer;
            for(int k = 0; k < qSize; k++)
            {
                currentPlayer = queue.Dequeue();
                if(currentPlayer.InStart()){
                    goalReached = true;
                    break;
                }

                newPosition = currentPlayer.CanMove('x', currentState);
                if(newPosition != null)
                    queue.Enqueue(newPosition);

                newPosition = currentPlayer.CanMove('<', currentState);
                if(newPosition != null)
                    queue.Enqueue(newPosition);

                newPosition = currentPlayer.CanMove('>', currentState);
                if(newPosition != null)
                    queue.Enqueue(newPosition);

                newPosition = currentPlayer.CanMove('^', currentState);
                if(newPosition != null)
                    queue.Enqueue(newPosition);

                newPosition = currentPlayer.CanMove('v', currentState);
                if(newPosition != null)
                    queue.Enqueue(newPosition);
            }
        }

        //Last trip        
        count--;
        stateIndex--;
        valleyStates =  generateAllValleyStates(valleyGround);
        valleyStates[stateIndex].OccupyIfPossible(0, 1);
        queue.Clear();
        queue.Enqueue(new Player(0, 1));


        goalReached = false;
        for(; queue.Count > 0 && !goalReached; count++)
        {
            stateIndex = (stateIndex == valleyStates.Length-1?0:stateIndex+1);
            ValleyState currentState = valleyStates[stateIndex];

            int qSize = queue.Count;
            Player? newPosition = null, currentPlayer;
            for(int k = 0; k < qSize; k++)
            {
                currentPlayer = queue.Dequeue();
                if(currentPlayer.InGoal()){
                    goalReached = true;
                    break;
                }

                newPosition = currentPlayer.CanMove('x', currentState);
                if(newPosition != null)
                    queue.Enqueue(newPosition);

                newPosition = currentPlayer.CanMove('<', currentState);
                if(newPosition != null)
                    queue.Enqueue(newPosition);

                newPosition = currentPlayer.CanMove('>', currentState);
                if(newPosition != null)
                    queue.Enqueue(newPosition);

                newPosition = currentPlayer.CanMove('^', currentState);
                if(newPosition != null)
                    queue.Enqueue(newPosition);

                newPosition = currentPlayer.CanMove('v', currentState);
                if(newPosition != null)
                    queue.Enqueue(newPosition);
            }
        }


        count--;
        if(goalReached){
            Console.WriteLine(count);
        }else{
            Console.WriteLine("UNREACHABLE!!!!!!!???????????");
        }

    }


}