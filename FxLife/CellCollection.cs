using System;
using System.IO;
using System.Text;

namespace fxLife
{
    class CellCollection
    {
        public CellCollection(int size)
        {
            _size = size;
        }
        public void ManualInitializeCells()
        {
            _cells = new CellOfLife[_size, _size];
            _nextGeneration = new bool[_size, _size];

            for (int row = 0; row < _size; row++)
            {
                for (int column = 0; column < _size; column++)
                {
                    _cells[row, column] = new CellOfLife();
                }
            }
        }
        public void AutoInitializeCells()
        {
            _cells = new CellOfLife[_size, _size];
            _nextGeneration = new bool[_size, _size];
            Random rn = new Random();
            for (int row = 0; row < _size; row++)
            {
                for (int column = 0; column < _size; column++)
                {
                    _cells[row, column] = new CellOfLife();
                    if (rn.NextDouble() <= 0.3) _cells[row, column].IsAlive = true;
                }
            }
        }

        public string SaveData
        {
            get
            {
                StringBuilder builder = new StringBuilder(2600);
                foreach (var cell in _cells)
                    if (cell.IsAlive) builder.Append('1');
                    else builder.Append('0');
                return builder.ToString();
            }
        }
        public void LoadFromString(string data)
        {
                ManualInitializeCells();
                int counter = 0;
                for (int row = 0; row < _size; row++)
                    for (int column = 0; column < _size; column++)
                    {
                        if (data[counter] == '1')
                            _cells[row, column].IsAlive = true;
                        counter++;
                    }
        }

        public void ResetCells()
        {
            for (int row = 0; row < _size; row++)
                for (int column = 0; column < _size; column++)
                    _cells[row, column].IsAlive = false;
        }
        public CellOfLife this[int row, int column]
        {
            get
            {
                if (row < 0 || row >= _size ||
                   column < 0 || column >= _size)
                {
                    return new CellOfLife();
                }

                return _cells[row, column];
            }
        }
        public int Size
        {
            get { return _size; }
            set { _size = value; }
        }
        public bool EndUpdateLife()
        {
            bool endgame = true;
            for (int row = 0; row < _size; row++)
            {
                for (int column = 0; column < _size; column++)
                {
                    int neighbors = CountNeighbors(row, column);

                    if (!_cells[row, column].IsAlive && neighbors == 3)
                    {
                        _nextGeneration[row, column] = true;
                    }
                    else if (_cells[row, column].IsAlive && (neighbors == 3 || neighbors == 2))
                    {
                        _nextGeneration[row, column] = true;
                    }
                    else
                    {
                        _nextGeneration[row, column] = false;
                    }
                }
            }
            if (HaveChanges()) endgame = false;
            for (int row = 0; row < _size; row++)
            {
                for (int column = 0; column < _size; column++)
                {
                    _cells[row, column].IsAlive = _nextGeneration[row, column];
                }
            }
            if (AllDead()) endgame = true;
            return endgame;
        }
        private bool HaveChanges()
        {
            for (int row = 0; row < _size; row++)
                for (int column = 0; column < _size; column++)
                    if (_nextGeneration[row, column] != _cells[row, column].IsAlive)
                        return true;
            return false;

        }
            private bool AllDead()
        {
            for (int row = 0; row < _size; row++)
                for (int column = 0; column < _size; column++)
                    if (_cells[row, column].IsAlive == true)
                        return false;
            return true;

        }
            private int CountNeighbors(int row, int column)
        {
            int neighbors = 0;
            if (_cells[WrapMinusOne(row), WrapMinusOne(column)].IsAlive)
                neighbors++;
            if (_cells[WrapMinusOne(row), column].IsAlive)
                neighbors++;
            if (_cells[WrapMinusOne(row), WrapPlusOne(column)].IsAlive)
                neighbors++;
            if (_cells[row, WrapMinusOne(column)].IsAlive)
                neighbors++;
            if (_cells[row, WrapPlusOne(column)].IsAlive)
                neighbors++;
            if (_cells[WrapPlusOne(row), WrapMinusOne(column)].IsAlive)
                neighbors++;
            if (_cells[WrapPlusOne(row), column].IsAlive)
                neighbors++;
            if (_cells[WrapPlusOne(row), WrapPlusOne(column)].IsAlive)
                neighbors++;
            return neighbors;
        }

        private int WrapMinusOne(int value)
        {
            if (value == 0)
                value = _size;
            return value - 1;
        }
        private int WrapPlusOne(int value)
        {
            if (value >= _size - 1)
                value = -1;
            return value + 1;
        }

        private int _size;
        private CellOfLife[,] _cells;
        private bool[,] _nextGeneration;
    }
}
