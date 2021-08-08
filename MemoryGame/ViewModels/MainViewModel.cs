using MemoryGame.Const;
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

        private string message;
        // トランププロパティ
        public string Message
        {
            get { return message; }
            set { SetProperty(ref message, value); }
        }

        public MainViewModel()
        {
            Initialize();
        }

        private void Initialize()
        {
            // メッセージを設定
            Message = MessageConst.USER_PROSESSING_MESSAGE;

            // トランプを生成
            CreateTrump();

            // シャッフル
            Trump = new ObservableCollection<TrumpModel>(Trump.OrderBy(i => Guid.NewGuid()).ToArray());
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
                    BackImage = "pack://application:,,,/Resources/card_back.png",
                    ClickTrumpCommand = new DelegateCommand<TrumpModel>(ClickTrumpExecute),
                    IsBack = true,
                    IsVisible = true,
                    IsHitTestVisible = true
                };

                // 最初の表示状態は裏に設定
                trumpModel.NowImage = trumpModel.BackImage;

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

                // 表の画像を設定
                trumpModel.FrontImage = string.Format("pack://application:,,,/Resources/card_{0}_{1}.png", trumpModel.Type, j);

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
            // クリックしたトランプの裏判定
            if (clickTrump.IsBack)
            {
                // 表にする
                OpenCard(clickTrump);

                if (Trump.Where(t => !t.IsBack).Count() == 2)
                {
                    Task<bool> t = CheckTrump();

                    ComputerProcessing(t);
                }
            }
        }

        private void OpenCard(TrumpModel clickTrump)
        {
            // 表にする
            clickTrump.IsBack = false;

            // 表の画像を設定
            clickTrump.NowImage = clickTrump.FrontImage;
        }

        private Task<bool> CheckTrump()
        {
            return Task.Run(() =>
            {
                bool isSuccess = false;
                SetTrumpDisable();

                // 2枚目のトランプが裏になるまで待機
                Thread.Sleep(1500);

                var cards = Trump.Where(t => !t.IsBack).ToList();

                // 番号の比較
                if (cards[0].Number == cards[1].Number)
                {
                    // 一致したため非表示にする
                    cards[0].IsVisible = false;
                    cards[1].IsVisible = false;
                    cards[0].IsBack = true;
                    cards[1].IsBack = true;

                    isSuccess = true;
                }
                else
                {
                    // 不一致のため、裏にする
                    cards[0].IsBack = true;
                    cards[1].IsBack = true;
                    cards[0].NowImage = cards[0].BackImage;
                    cards[1].NowImage = cards[1].BackImage;
                }

                SetTrumpEnable();

                return isSuccess;
            });
        }

        private Task ComputerProcessing(Task<bool> userProcessing)
        {
            return Task.Run(() =>
            {
                // ユーザ操作待機
                userProcessing.Wait();

                // ユーザが失敗したか判定
                if (!userProcessing.Result)
                {
                    bool isSuccess = true;

                    // 失敗するまで繰り返す
                    while (isSuccess)
                    {
                        SetTrumpDisable();

                        Message = MessageConst.CP_PROSESSING_MESSAGE;

                        var getTrump = Trump.OrderBy(elem => Guid.NewGuid()).Where(c => c.IsBack && c.IsVisible);
                        int i = 0;
                        foreach (var card in getTrump)
                        {
                            if (i == 2)
                            {
                                break;
                            }
                            Thread.Sleep(1000);

                            card.IsBack = false;
                            card.NowImage = card.FrontImage;
                            i++;
                        }

                        Task<bool> t = CheckTrump();
                        t.Wait();

                        Message = MessageConst.USER_PROSESSING_MESSAGE;

                        SetTrumpEnable();

                        isSuccess = t.Result;
                    }
                }
            });
        }

        private void SetTrumpEnable()
        {
            foreach (TrumpModel trumpModel in Trump)
            {
                // クリックを有効化
                trumpModel.IsHitTestVisible = true;
            }
        }

        private void SetTrumpDisable()
        {
            foreach (TrumpModel trumpModel in Trump)
            {
                // 処置中はクリックを無効化
                trumpModel.IsHitTestVisible = false;
            }
        }
    }
}
