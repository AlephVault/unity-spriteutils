using System;
using AlephVault.Unity.SpriteUtils.Authoring.Types;
using UnityEngine;


namespace AlephVault.Unity.SpriteUtils
{
    namespace Samples
    {
        [RequireComponent(typeof(SampleSpriteGridFactory))]
        public class SampleSpriteGridAssigner : MonoBehaviour
        {
            private SampleSpriteGridFactory factory;

            [SerializeField]
            private SampleSpriteGridApplier[] appliers;

            private int preparedIndex = -1;
            
            private void Awake()
            {
                factory = GetComponent<SampleSpriteGridFactory>();
                if (appliers.Length < 4)
                {
                    Destroy(gameObject);
                    throw new ArgumentException("Please set at least 4 appliers");
                }
            }

            private void Update()
            {
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    preparedIndex = 0;
                }                
                else if (Input.GetKeyDown(KeyCode.W))
                {
                    preparedIndex = 1;
                }
                else if (Input.GetKeyDown(KeyCode.E))
                {
                    preparedIndex = 2;
                }
                else if (Input.GetKeyDown(KeyCode.R))
                {
                    preparedIndex = 3;
                }
                else if (Input.GetKeyDown(KeyCode.T))
                {
                    preparedIndex = -1;
                }
                else
                {
                    bool found = false;
                    for(KeyCode key = KeyCode.Alpha0; key <= KeyCode.Alpha9; key++)
                    {
                        if (Input.GetKeyDown(key) && preparedIndex != -1)
                        {
                            SampleSpriteGridApplier obj = appliers[preparedIndex];
                            preparedIndex = -1;
                            obj.UseSpriteGrid(factory.Get(key - KeyCode.Alpha0));
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        if (Input.GetKeyDown(KeyCode.Minus) && preparedIndex != -1)
                        {
                            SampleSpriteGridApplier obj = appliers[preparedIndex];
                            preparedIndex = -1;
                            obj.ReleaseSpriteGrid();
                        }
                    }
                }
            }
        }
    }
}
