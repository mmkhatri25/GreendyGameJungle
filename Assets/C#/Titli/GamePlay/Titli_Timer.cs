using System;
using System.Collections;
using UnityEngine;
using Titli.Utility;
using Newtonsoft.Json;
using System.Collections.Generic;
using Titli.UI;
using Titli.ServerStuff;
using UnityEngine.UI;
using KhushbuPlugin;
using TMPro;

namespace Titli.Gameplay
{
    public class Titli_Timer : MonoBehaviour
    {
        public static Titli_Timer Instance;
        public GameObject waitForBetScreen;
        int bettingTime = 30;
        int timeUpTimer = 5;
        // int waitTimer = 3;
        public Action onTimeUp;
        public Action onCountDownStart;
        public Action startCountDown;
        public static gameState gamestate;
       public TMP_Text countdownTxt, waittext;
        public Text todaywin;
        public GameObject WheelToRotate;
        // [SerializeField] TMP_Text messageTxt;
        private void Awake()
        {
            Instance = this;
            
            //Titli_ServerRequest.instance.socket.Emit(Events.winnerList);
        }
        void onWinnerListREceived()
        {

        }
        void Start()
        {
            // Titli_UiHandler.Instance.ShowMessage("please wait for next round...");
            gamestate = gameState.cannotBet;
            // onTimeUp?.Invoke();
            // onTimeUp();
            if(is_a_FirstRound)
            {
                print("this is fists time  - "+ is_a_FirstRound);
                // Titli_CardController.Instance._winNo = true;
                // for(int i = 0; i < Titli_CardController.Instance.TableObjs.Count; i++)
                // {
                //     Titli_CardController.Instance.TableObjs[i].GetComponent<BoxCollider2D>().enabled = false;
                // }
            }
        }    

        public void OnCurrentTime(object data = null)
        {
            // is_a_FirstRound = true;
            //if (is_a_FirstRound)
            //{
            //    waitForBetScreen.SetActive(true);
            //}
            //else
            {
            waitForBetScreen.SetActive(false);
           // onTimeUp();
           
            InitialData init = new InitialData();
            try
            {
                init = Utility.Utility.GetObjectOfType<InitialData>(data.ToString());

            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
                //if (init.gametimer > 5)
                //{
                // Titli_UiHandler.Instance.lessthanFiveSec = false;
                //print("Here getting gametimer... - "+ init.gametimer + " round first "+ is_a_FirstRound);
                // here is setting the stop bet....
                if (init.gametimer >=2)
                {
                   is_a_FirstRound = false;
                //print("Here setting true"+ init.gametimer + " round first "+ is_a_FirstRound);

                }
                OnTimerStart(init.gametimer);
                waitForBetScreen.SetActive(false);
                
            //}
            //else if (init.gametimer < 5)
            //{
            //    Titli_UiHandler.Instance.lessthanFiveSec = true;
            //    onTimeUp();
            //    StartCoroutine(currentTimer(init.gametimer));
            //}
                todaywin.text = init.currentWin.ToString();
                
         }
       }

        IEnumerator currentTimer(int currentGametimer)
        {
            for (int i = currentGametimer; i >= 0; i--)
            {
                // Titli_UiHandler.Instance.ShowMessage("please wait for next round... " + i.ToString() );
                yield return new WaitForSecondsRealtime(1f);
            }
        }

        public void OnTimerStart(int time)
        {
            // if (is_a_FirstRound)
            // {
            //     Titli_UiHandler.Instance.HideMessage();
            // }
            // is_a_FirstRound = false;
            Titli_UiHandler.Instance.ResetUi();
            Titli_CardController.Instance._startCardBlink = false;
            Titli_CardController.Instance._canPlaceBet = true;
            // Titli_UiHandler.Instance.ResetUi();
            StartCoroutine(timerStartCountdown(time));
            for(int i = 0; i < Titli_CardController.Instance.TableObjs.Count; i++)
            {
                Titli_CardController.Instance.TableObjs[i].GetComponent<BoxCollider2D>().enabled = true;
            }

            // StopCoroutines();
        }

        //this will run once it connected to the server
        //it will carry the time and state of server
        IEnumerator timerStartCountdown(int time)
        {
            onCountDownStart?.Invoke();
            gamestate = gameState.canBet;
            Titli_CardController.Instance._canPlaceBet = true;
            for (int i = time; i >= 0; i--)
            {
                if (i == 1)
                {
                    Debug.Log("1 countdown - " + i);

                    startCountDown?.Invoke();
                    //countdownTxt.text = "wait..";
                    //print("here countdown become 0 ...");
                countdownTxt.text = i.ToString();


                }
                else
                {
                    //waittext.gameObject.SetActive(false);
                countdownTxt.text = i.ToString();
                }

                if (i <= 0)
                {
                    Debug.Log("2 countdown - "+ i);
                    countdownTxt.text = "";

                    //waittext.gameObject.SetActive(true);
                    //countdownTxt.gameObject.SetActive(false);
                }
                // Debug.Log("Timer:" +i);
                if (i > 5)
                    Titli_Timer.Instance.waitForBetScreen.SetActive(false);
                yield return new WaitForSecondsRealtime(1f);
            }
            
            // Titli_ServerResponse.Instance.TimerUpFunction();
            onTimeUp?.Invoke();

        }


        public void OnTimeUp(object data)
        {
            if (is_a_FirstRound) return;
            
            Titli_CardController.Instance._canPlaceBet = false;
                waitForBetScreen.SetActive(false);
            
            // for(int i = 0; i < Titli_CardController.Instance.TableObjs.Count; i++)
            // {
            //     Titli_CardController.Instance.TableObjs[i].GetComponent<BoxCollider2D>().enabled = false;
            // }
            // StopCoroutines();
            StartCoroutine(TimeUpCountdown());
        }

        IEnumerator TimeUpCountdown(int time = -1)
        {
            gamestate = gameState.cannotBet;
            onTimeUp?.Invoke();
            Titli_CardController.Instance._startCardBlink = true;
            Titli_CardController.Instance._canPlaceBet = false;

            // foreach(var item in Titli_CardController.Instance._cardsImage)
            // {
            //     item.GetComponent<Button>().interactable = false;
            // }
            // StartCoroutine(Titli_CardController.Instance.CardsBlink());
            StartCoroutine(Rotate360Degrees());

            for (int i = time != -1 ? time : timeUpTimer; i >= 0; i--)
            {
                // messageTxt.text = "Time Up";
                //countdownTxt.text = "Time Up";//i.ToString();
                print("here counting ... " +countdownTxt.text );

                yield return new WaitForSecondsRealtime(1f);
            }
            countdownTxt.text = "";

        }
        public AnimationCurve rotationCurve;
        private IEnumerator Rotate360Degrees()
        {
            float totalRotation = 0f;
            float duration = 5f; // Total time for the rotation
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                // Calculate the percentage of completion
                float t = elapsedTime / duration;

                // Get the speed factor from the animation curve
                float speedFactor = rotationCurve.Evaluate(t);

                // Calculate rotation for this frame
                float rotationThisFrame = speedFactor * 360 * Time.deltaTime;

                // Apply rotation to the GameObject
                WheelToRotate.transform.Rotate(0, 0, -rotationThisFrame);

                // Update total rotation
                totalRotation += rotationThisFrame;

                // Update elapsed time
                elapsedTime += Time.deltaTime;

                // Wait until next frame
                yield return null;
            }

            // Ensure final rotation is exactly 0 degrees (smooth stop)
            //float remainingRotation = 360f - (totalRotation % 360f);
            //float smoothStopDuration = 0.2f; // Duration for the smooth stop
            //float smoothStopElapsedTime = 0f;

            //while (smoothStopElapsedTime < smoothStopDuration)
            //{
            //    float t = smoothStopElapsedTime / smoothStopDuration;
            //    float rotationThisFrame = Mathf.Lerp(remainingRotation, 0, t) * Time.deltaTime / smoothStopDuration;
            //    WheelToRotate.transform.Rotate(0, 0, -rotationThisFrame);
            //    smoothStopElapsedTime += Time.deltaTime;
            //    yield return null;
            //}

            //// Ensure the final adjustment to exactly 0 degrees if there's any remaining small offset
            //WheelToRotate.transform.Rotate(0, 0, -remainingRotation);
        }


        //Local Timer Start countdown
        // IEnumerator TimerStartCountDown(int timer = 25)
        // {
        //     Debug.Log("timer count down start");
        //     gamestate = gameState.canBet;

        //     // Titli_CardController.Instance._winNo = true;
        //     for(int i = timer; i >= 0; i--)
        //     {
        //         if (i == 1)
        //         {
        //             startCountDown?.Invoke();
        //         }
        //         messageTxt.text = "Bettting Time";
        //         countdownTxt.text = i.ToString();
        //         yield return new WaitForSecondsRealtime(1f);
        //     }
        //     Titli_CardController.Instance._canPlaceBet = true;
        //     StartCoroutine(TimeUpCountDown());
        //     onTimeUp?.Invoke();
        // }



        //Local Timer timeUp Countdown
        // IEnumerator TimeUpCountDown(int timer = 5)
        // {
        //     gamestate = gameState.cannotBet;
        //     onTimeUp?.Invoke();

        //     Titli_CardController.Instance._startCardBlink = true;
        //     Titli_CardController.Instance._canPlaceBet = false;
        //     foreach(var item in Titli_CardController.Instance._cardsImage)
        //     {
        //         item.GetComponent<Button>().interactable = false;
        //     }
        //     // Titli_ServerResponse.Instance.OnWinFunction();
        //     Titli_CardController.Instance.CardBlink_coroutine = Titli_CardController.Instance.CardsBlink();
        //     StartCoroutine(Titli_CardController.Instance.CardsBlink());
        //     // Titli_CardController.Instance._winNo = true;
        //     for(int i = timer; i >= 0; i--)
        //     {
        //         if(i == 2)
        //         {
        //             Titli_CardController.Instance._winNo = true;
        //             // Titli_ServerResponse.Instance.OnWinFunction();
        //         }
        //         messageTxt.text = "Time Up";
        //         countdownTxt.text = i.ToString();
        //         yield return new WaitForSecondsRealtime(2f);
        //     }
        // }

        // IEnumerator WaitCountdown(int time = -1)
        // {
        //     gamestate = gameState.wait;
        //     Titli_CardController.Instance._canPlaceBet = false;
        //     for (int i = time != -1 ? time : waitTimer; i >= 0; i--)
        //     {
        //         messageTxt.text = "Wait Time";
        //         countdownTxt.text = i.ToString();
        //         yield return new WaitForSecondsRealtime(1f);
        //     }

        // }




        // public void OnTimerStart()
        // {
        //     if (is_a_FirstRound)
        //     {
        //         Titli_UiHandler.Instance.HideMessage();
        //     }
        //     is_a_FirstRound = false;
        //     Titli_CardController.Instance._startCardBlink = false;
        //     Titli_CardController.Instance._canPlaceBet = true;

        //     StopCoroutines();
        //     Stop_CountDown = TimerStartCountDown();
        //     Debug.Log("timer start");
        //     // StartCoroutine(TimerStartCountDown());
        //     StartCoroutine(Stop_CountDown);
        //     // StartCoroutine(Countdown());
        // }



        // public void OnTimeUp()
        // {
        //     if (is_a_FirstRound) return;
        //     Titli_CardController.Instance._canPlaceBet = false;
        //     StopCoroutines();
        //     StopCoroutine(Stop_CountDown);
        //     StartCoroutine(onTimeUpcountDown());
        // }

        // public void OnWait(object data)
        // {            
        //     StopCoroutines();
        //     // StartCoroutine(StartDragonAnim());           
        //     if (is_a_FirstRound) return;
        //     // StartCoroutine(WOF_UiHandler.Instance.StartImageAnimation());
        //     StopCoroutines();
        //     StartCoroutine(WaitCountdown());
        // }
        public bool is_a_FirstRound = true;
        

        // public void StopCoroutines()
        // {
        //     StopCoroutine(Countdown());
        //     StopCoroutine(TimeUpCountdown());
        //     StopCoroutine(WaitCountdown());
        // }
    }
    [Serializable]
    public class CurrentTimer
    {
        public gameState gameState;
        public int timer;
        public List<int> lastWins;
        public int LeftBets;
        public int MiddleBets;
        public int RightBets;
    }
    public enum gameState
    {
        canBet = 0,
        cannotBet = 1,
        wait = 2,
    }

    public class InitialData
    {
        public List<int> previousWins;
        public List<BotsBetsDetail> botsBetsDetails;
        public string balance;
        public int gametimer;
        public int userDailyWin;
        public double currentWin;
    }
}
