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

        private bool isProcessing;
        // 処理中フラグ
        public bool IsProcessing
        {
            get { return isProcessing; }
            set { SetProperty(ref isProcessing, value, ChangeProcessing); }
        }


        private ComputerModel computer;

        public MainViewModel()
        {
            Initialize();
        }

        private void Initialize()
        {
            computer = new ComputerModel();
            computer.Level = 1;
            computer.Storage = new ObservableCollection<TrumpModel>();

            // メッセージを設定
            Message = MessageConst.USER_PROSESSING_MESSAGE;

            // トランプを生成
            CreateTrump();

            // シャッフル
            Trump = new ObservableCollection<TrumpModel>(Trump.OrderBy(i => Guid.NewGuid()).ToArray());
        }

        private void  ChangeProcessing()
        {
            if (IsProcessing)
            {
                // カードの操作を有効化
                SetTrumpDisable();

            }
            else
            {
                // カードの操作を有効化
                SetTrumpEnable();
            }
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

                // 表のカード枚数判定
                if (Trump.Where(t => !t.IsBack).Count() == 2)
                {
                    IsProcessing = true;
                    // 2枚だったら判定を行う
                    Task<bool> t = CheckTrump();

                    // コンピュータの処理
                    ComputerProcessing(t);
                }
            }
        }

        private void OpenCard(TrumpModel trump)
        {
            // 表にする
            trump.IsBack = false;

            // 表の画像を設定
            trump.NowImage = trump.FrontImage;

            if (computer.Storage.Contains(trump))
            {
                computer.Storage.Remove(trump);
            }
            // 記憶する
            computer.Storage.Add(trump);
        }

        private void OpenCard(TrumpModel targetCard1, TrumpModel targetCard2)
        {
            Thread.Sleep(1000);
            OpenCard(targetCard1);
            Thread.Sleep(1000);
            OpenCard(targetCard2);
        }

        private void BackCard(TrumpModel trump)
        {
            // 表にする
            trump.IsBack = true;

            // 表の画像を設定
            trump.NowImage = trump.BackImage;

            if (computer.Storage.Contains(trump))
            {
                computer.Storage.Remove(trump);
            }
            // 記憶する
            computer.Storage.Add(trump);
        }


        private Task<bool> CheckTrump()
        {
            return Task.Run(() =>
            {
                bool isSuccess = false;

                // 2枚目のトランプが表になるまで待機
                Thread.Sleep(1500);

                // 表のカードを抽出
                var cards = Trump.Where(t => !t.IsBack).ToList();

                // 番号の比較
                if (cards[0].Number == cards[1].Number)
                {
                    // 一致したため非表示にする
                    cards[0].IsVisible = false;
                    cards[1].IsVisible = false;

                    // 裏にする
                    BackCard(cards[0]);
                    BackCard(cards[1]);

                    // 成功
                    isSuccess = true;

                    // 記憶領域から削除
                    computer.Storage.Remove(cards[0]);
                    computer.Storage.Remove(cards[1]);
                }
                else
                {
                    // 不一致のため、裏にする
                    BackCard(cards[0]);
                    BackCard(cards[1]);
                }

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
                    Message = MessageConst.CP_PROSESSING_MESSAGE;

                    // 記憶したカードから一致するカードを抽出
                    for (int i = 0; i < computer.Storage.Count; i++)
                    {
                        for (int j = i + 1; j < computer.Storage.Count; j++)
                        {
                            if (computer.Storage[i].Number == computer.Storage[j].Number)
                            {
                                // 対象のカード
                                var targetCard1 = computer.Storage[i];
                                var targetCard2 = computer.Storage[j];

                                OpenCard(targetCard1, targetCard2);

                                Task<bool> t = CheckTrump();
                                t.Wait();

                                break;
                            }
                        }
                    }

                    bool isSuccess = true;

                    // 失敗するまで繰り返す
                    while (isSuccess)
                    {
                        var getTrump = Trump.OrderBy(elem => Guid.NewGuid()).Where(c => c.IsBack && c.IsVisible).ToList();
                        var targetCard1 = getTrump[0];
                        var targetCard2 = getTrump[1];

                        // 記憶領域から存在するカードを取得
                        var targetCards2 = computer.Storage.Where(c => (c.Number == targetCard1.Number) && c.IsBack && c.IsVisible && (c.Type != targetCard1.Type)).ToList();
                        if (targetCards2.Count() != 0)
                        {
                            // 存在したら2枚目を設定
                            targetCard2 = targetCards2[0];
                        }

                        OpenCard(targetCard1, targetCard2);

                        Task<bool> t = CheckTrump();
                        t.Wait();

                        isSuccess = t.Result;
                    }

                    Message = MessageConst.USER_PROSESSING_MESSAGE;
                }

                IsProcessing = false;

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
