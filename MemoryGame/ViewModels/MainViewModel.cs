using MemoryGame.Models;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MemoryGame.ViewModels
{
    public class MainViewModel : BindableBase
    {
        // トランプフィールド
        private ObservableCollection<TrumpModel> trump;
        // トランププロパティ
        public ObservableCollection<TrumpModel> Trump
        {
            get { return trump; }
            set { SetProperty(ref trump, value); }
        }

        // 開いた1枚目のトランプ保持用フィールド
        private TrumpModel openTrump;

        public MainViewModel()
        {
            // トランプを生成
            CreateTrump();

            // シャッフル
            Trump = new ObservableCollection<TrumpModel>( Trump.OrderBy(i => Guid.NewGuid()).ToArray());
        }
        private void CreateTrump()
        {
            // トランプを生成
            Trump = new ObservableCollection<TrumpModel>();
            int j = 1;
            // トランプ数分処理(ジョーカーは含まない)
            for (int i = 1; i <= 52; i++)
            {
                // 1枚のトランプ情報を生成
                TrumpModel trumpModel = new TrumpModel()
                {
                    Number = j.ToString(),
                    FrontImage = "pack://application:,,,/Resources/card_front.png",
                    ClickTrumpCommand = new DelegateCommand<TrumpModel>(ClickTrumpExecute),
                    IsFront = true,
                    IsVisible = true
                };

                // 最初の表示状態は裏に設定
                trumpModel.NowImage = trumpModel.FrontImage;

                if (i <= 13)
                {
                    // スペード
                    trumpModel.Type = "spade";
                }
                else if (i <= 26)
                {
                    // ハート
                    trumpModel.Type = "heart";
                }
                else if (i <= 39)
                {
                    // クラブ
                    trumpModel.Type = "club";
                }
                else if (i <= 52)
                {
                    // ダイヤ
                    trumpModel.Type = "diamond";
                }

                // 裏の画像を設定
                trumpModel.BackImage = string.Format("pack://application:,,,/Resources/card_{0}_{1}.png", trumpModel.Type, j);

                // トランプに追加
                Trump.Add(trumpModel);

                if (j == 13)
                {
                    // 次の種類になるので1にする
                    j = 1;
                }
                else
                {
                    // 次の数値へ
                    j++;
                }
            }
        }

        private void ClickTrumpExecute(TrumpModel clickTrump)
        {
            // クリックしたトランプの表判定
            if (clickTrump.IsFront)
            {
                // 裏にする
                clickTrump.IsFront = false;

                // 裏の画像を設定
                clickTrump.NowImage = clickTrump.BackImage;

                // すでにオープン済みのトランプがあるか判定
                if (openTrump == null)
                {
                    // なければ、クリックしたトランプをオープンしたトランプとして保持
                    openTrump = clickTrump;
                }
                else
                {
                    // オープンしているトランプとクリックしたトランプを比較
                    CheckTrump(clickTrump);
                }
            }
        }

        private Task CheckTrump(TrumpModel clickTrump)
        {
            return Task.Run(() =>
            {
                // 2枚目のトランプが裏になるまで待機
                Thread.Sleep(1500);

                // 番号の比較
                if (openTrump.Number == clickTrump.Number)
                {
                    // 一致したため非表示にする
                    openTrump.IsVisible = false;
                    clickTrump.IsVisible = false;
                }
                else
                {
                    // 不一致のため、裏にする
                    clickTrump.IsFront = true;
                    openTrump.IsFront = true;
                    clickTrump.NowImage = clickTrump.FrontImage;
                    openTrump.NowImage = openTrump.FrontImage;
                }

                openTrump = null;
            });
        }
    }
}
