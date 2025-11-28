class Program
{
    static char[] board = { '1', '2', '3', '4', '5', '6', '7', '8', '9' };
    static int player = 1;

    static void Main()
    {
        int choice;
        char mark;
        do
        {
            Console.Clear();
            DrawBoard();
            player = (player % 2 == 0) ? 2 : 1;
            Console.WriteLine($"Играч {player}, въведи число (1-9): ");
            string? input = Console.ReadLine();

            if (!int.TryParse(input, out choice) || choice < 1 || choice > 9)
            {
                Console.WriteLine("Невалиден вход! Натисни Enter...");
                Console.ReadLine();
                continue;
            }

            mark = (player == 1) ? 'X' : 'O';

            if (board[choice - 1] != 'X' && board[choice - 1] != 'O')
                board[choice - 1] = mark;
            else
            {
                Console.WriteLine("Полето е заето! Натисни Enter...");
                Console.ReadLine();
                continue;
            }

            int result = CheckWin();
            if (result == 1)
            {
                Console.Clear();
                DrawBoard();
                Console.WriteLine($"Играч {player} печели!");
                break;
            }
            else if (result == -1)
            {
                Console.Clear();
                DrawBoard();
                Console.WriteLine("Равенство!");
                break;
            }

            player++;
        } while (true);
    }

    static void DrawBoard()
    {
        Console.WriteLine("Морски шах");
        Console.WriteLine($" {board[0]} | {board[1]} | {board[2]} ");
        Console.WriteLine("---|---|---");
        Console.WriteLine($" {board[3]} | {board[4]} | {board[5]} ");
        Console.WriteLine("---|---|---");
        Console.WriteLine($" {board[6]} | {board[7]} | {board[8]} ");
    }

    static int CheckWin()
    {
        int[][] winCombos = new int[][]
        {
            new int[] {0,1,2}, new int[] {3,4,5}, new int[] {6,7,8},
            new int[] {0,3,6}, new int[] {1,4,7}, new int[] {2,5,8},
            new int[] {0,4,8}, new int[] {2,4,6}
        };

        foreach (var combo in winCombos)
        {
            if (board[combo[0]] == board[combo[1]] &&
                board[combo[1]] == board[combo[2]])
                return 1;
        }

        foreach (char c in board)
            if (c != 'X' && c != 'O') return 0;

        return -1;
    }
}
