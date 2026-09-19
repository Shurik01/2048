using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace _2048
{
    // шанс выпадения двойки - 90%, четверки - 10%
    class Matrix2048
    {
        private Random random = new Random();
        private int[,] matrix = new int[4, 4];

        public bool SpawnNewNum() {
            List<(int row, int col)> emptyCells = new List<(int, int)>();
            random.Next();
            for (int r = 0; r < 4; r++) {
                for (int c = 0; c < 4; c++) {
                    if (matrix[r, c] == 0) { 
                        emptyCells.Add((r, c));
                    }
                }
            }
            if (emptyCells.Count == 0) {
                return false;
            }
            int randomIndex = random.Next(emptyCells.Count);
            var (row, col) = emptyCells[randomIndex];
            if (random.NextDouble() < 0.1)
            {
                matrix[row, col] = 4;
                return true;
            }
            else {
                matrix[row, col] = 2;
                return true;
            }

        }

        public bool IsGameOver()
        {
            Matrix2048 tempMatrix = new Matrix2048();
            tempMatrix.matrix = (int[,])matrix.Clone();
            if (tempMatrix.ToLeft() == false && tempMatrix.ToRight() == false && tempMatrix.ToUp() == false && tempMatrix.ToDown() == false)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public int GetValue(int row, int col) { 
            return matrix[row, col];
        }


        public bool ToLeft() {
            bool isMoved = false;
            List<int> row = new List<int>();
            for (int r = 0; r < 4; r++)
            {
                row.Clear();
                for (int c = 0; c < 4; c++)
                {
                    if (matrix[r, c] != 0)
                    {
                        row.Add(matrix[r, c]);
                    }
                }
                List<int> collapsedRow = new List<int>();
                for (int i = 0; i < row.Count; i++)
                {
                    if (i < row.Count - 1 && row[i] == row[i + 1])
                    {
                        collapsedRow.Add(row[i] * 2);
                        // анимация слияния
                        i++;
                    }
                    else
                    {
                        collapsedRow.Add(row[i]); 
                    }
                }

                for (int c = 0; c < 4; c++)
                {
                    int newValue = 0;
                    if (c < collapsedRow.Count)
                    {
                        newValue = collapsedRow[c];
                    }

                    if (matrix[r, c] != newValue)
                    {
                        isMoved = true;
                    }

                    matrix[r, c] = newValue;

                }
            }
            return isMoved;
        }


        public bool ToRight()
        {
            bool isMoved = false;
            List<int> row = new List<int>();
            for (int r = 0; r < 4; r++)
            {
                row.Clear();
                for (int c = 0; c < 4; c++)
                {
                    if (matrix[r, c] != 0)
                    {
                        row.Add(matrix[r, c]);
                    }
                }
                List<int> collapsedRow = new List<int>();
                for (int i = row.Count-1; i >= 0; i--)
                {
                    if (i > 0 && row[i] == row[i - 1])
                    {
                        collapsedRow.Add(row[i] * 2);
                        i--;
                    }
                    else
                    {
                        collapsedRow.Add(row[i]);
                    }
                }

                int k = 0;
                for (int c = 3; c >= 0; c--)
                {
                    int newValue = 0;
                    if (k < collapsedRow.Count)
                    {
                        newValue = collapsedRow[k];
                        k++;
                    }

                    if (matrix[r, c] != newValue)
                    {
                        isMoved = true;
                    }

                    matrix[r, c] = newValue;
                }
            }
            return isMoved;
        }


        public bool ToUp()
        {
            bool isMoved = false;
            List<int> col = new List<int>();
            for (int c = 0; c < 4; c++)
            {
                col.Clear();
                for (int r = 0; r < 4; r++)
                {
                    if (matrix[r, c] != 0)
                    {
                        col.Add(matrix[r, c]);
                    }
                }
                List<int> collapsedCol = new List<int>();
                for (int i = 0; i < col.Count; i++)
                {
                    if (i < col.Count - 1 && col[i] == col[i + 1])
                    {
                        collapsedCol.Add(col[i] * 2);
                        i++;
                    }
                    else
                    {
                        collapsedCol.Add(col[i]);
                    }
                }

                for (int r = 0; r < 4; r++)
                {
                    int newValue = 0;
                    if (r < collapsedCol.Count)
                    {
                        newValue = collapsedCol[r];
                    }

                    if (matrix[r, c] != newValue)
                    {
                        isMoved = true;
                    }

                    matrix[r, c] = newValue;
                }
            }
            return isMoved;
        }

        public bool ToDown()
        {
            bool isMoved = false;
            List<int> col = new List<int>();
            for (int c = 0; c < 4; c++)
            {
                col.Clear();
                for (int r = 0; r < 4; r++)
                {
                    if (matrix[r, c] != 0)
                    {
                        col.Add(matrix[r, c]);
                    }
                }
                List<int> collapsedCol = new List<int>();
                for (int i = col.Count-1; i >= 0; i--)
                {
                    if (i > 0 && col[i] == col[i - 1])
                    {
                        collapsedCol.Add(col[i] * 2);
                        i--;
                    }
                    else
                    {
                        collapsedCol.Add(col[i]);
                    }
                }

                int k = 0;
                for (int r = 3; r >= 0; r--)
                {
                    int newValue = 0;
                    if (k < collapsedCol.Count)
                    {
                        newValue = collapsedCol[k];
                        k++;
                    }

                    if (matrix[r, c] != newValue)
                    {
                        isMoved = true;
                    }

                    matrix[r, c] = newValue;
                }
            }
            return isMoved;
        }

        public void StartGame() {
            Array.Clear(matrix, 0, matrix.Length);
            SpawnNewNum();
            SpawnNewNum();
        }
    }
}
