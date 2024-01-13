using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameWaveTracker : MonoBehaviour
{
    public List<InGameWaveTrackerItem> items;
    public InGameWaveTrackerItem prefab;
    public ScrollMoveToTarget scroll;
    public Transform _panel;

    private InGameMapConfig _map;
    
    public void ParseData(InGameMapConfig _map)
    {
        this._map = _map;
        List<InGameEnemyWaveConfig> waves = _map._waveConfigs;
        items ??= new List<InGameWaveTrackerItem>();
        items.ForEach(x => x.gameObject.SetActive(false));
        InGameWaveTrackerItem item;
        for (int i = 0; i < waves.Count; i++)
        {
            if (i < this.items.Count)
            {
                item = this.items[i];
            }
            else
            {
                item = Instantiate(this.prefab, this._panel);
                this.items.Add(item);
            }

            item.gameObject.SetActive(true);
            item.ParseData(wave: i + 1, icon: WaveIconConfigs.Instance.GetSprite(waves[i]._type), isLastWave: i == waves.Count - 1);
        }

        this.UpdateCenterItem();
    }
    public void NewWave(int currentWave)
    {
        InGameWaveTrackerItem item = this.GetItem(currentWave-1);
        if (item != null)
            item.SetIsIn(false);

        this.UpdateCenterItem();
    }
    private void UpdateCenterItem()
    {
        InGameWaveTrackerItem item = this.GetCurrentStepItem();
        item.SetIsIn(true);
        if (item != null)
        {
            this.scroll.OnMoveToCenter(item.transform as RectTransform);
        }
    }
    public InGameWaveTrackerItem GetCurrentStepItem()
    {
        return GetItem(InGameManager.Instance.CurrentWaveIndex);
    }
    public InGameWaveTrackerItem GetItem(int index)
    {
        if (index >= 0 && index < items.Count)
            return items[index];
        return null;
    }
}
