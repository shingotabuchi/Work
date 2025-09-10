namespace echo17.EnhancedUI.EnhancedGrid.Demo_06
{
    using UnityEngine;
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
	/// This demo shows how you can have different cell types, each using different prefabs
	/// </summary>
    public class Controller : MonoBehaviour, IEnhancedGridDelegate
    {
        private enum CellType
        {
            A,
            B,
            C
        }

        public EnhancedGrid grid;
        public int numberOfCells;
        public Vector2 cellMinSize;
        public Vector2 cellMaxSize;
        public int randomSeed;

        // The three different cell type prefabs
        public GameObject cellPrefabA;
        public GameObject cellPrefabB;
        public GameObject cellPrefabC;

        private List<Model_Base> _data;

        void Awake()
        {
            UnityEngine.Random.InitState(randomSeed);

            Application.targetFrameRate = 60;
        }

        void Start()
        {
            grid.InitializeGrid(this);

            _data = new List<Model_Base>();

            // our data uses different model types, which all inherit from
            // the same base type
            Model_Base modelElement;

            for (var i = 0; i < numberOfCells; i++)
            {
                // alternate between the three model types

                switch (i % 3)
                {
                    case 0:
                        modelElement = new Model_1();
                        modelElement.cellType = Demo_06.CellType.A;
                        break;

                    case 1:
                        modelElement = new Model_2();
                        (modelElement as Model_2).value = UnityEngine.Random.Range(0f, 100f);
                        modelElement.cellType = Demo_06.CellType.B;
                        break;

                    case 2:
                        modelElement = new Model_1();
                        modelElement.cellType = Demo_06.CellType.C;
                        break;

                    default:
                        modelElement = new Model_1();
                        modelElement.cellType = Demo_06.CellType.A;
                        break;
                }

                modelElement.cellSize = new Vector2(
                                            UnityEngine.Random.Range(cellMinSize.x, cellMaxSize.x),
                                            UnityEngine.Random.Range(cellMinSize.y, cellMaxSize.y)
                                            );

                _data.Add(modelElement);
            }

            grid.RecalculateGrid();
        }

        public int GetCellCount(EnhancedGrid grid)
        {
            return _data.Count;
        }

        public GameObject GetCellPrefab(EnhancedGrid grid, int dataIndex)
        {
            // return the correct prefab based on the cell type stored in the model
            switch (_data[dataIndex].cellType)
            {
                case Demo_06.CellType.A: return cellPrefabA;
                case Demo_06.CellType.B: return cellPrefabB;
                case Demo_06.CellType.C: return cellPrefabC;
                default: return cellPrefabA;
            }
        }

        public CellProperties GetCellProperties(EnhancedGrid grid, int dataIndex)
        {
            return new CellProperties(_data[dataIndex].cellSize, 0f);
        }

        public void UpdateCell(EnhancedGrid grid, IEnhancedGridCell cell, int dataIndex, int repeatIndex, CellLayout cellLayout, GroupLayout groupLayout)
        {
            // update the appropriate cell view based on the cell type stored in the model
            switch (_data[dataIndex].cellType)
            {
                case Demo_06.CellType.A: (cell as View_A).UpdateCell(_data[dataIndex] as Model_1); break;
                case Demo_06.CellType.B: (cell as View_B).UpdateCell(_data[dataIndex] as Model_2); break;
                case Demo_06.CellType.C: (cell as View_C).UpdateCell(_data[dataIndex] as Model_1); break;
            }
        }
    }
}