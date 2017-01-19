﻿using System;
using System.Linq;
using System.Runtime.InteropServices;

using Microsoft.Office.Interop.PowerPoint;

using ARSnovaPPIntegration.Business.Model;

namespace ARSnovaPPIntegration.Presentation.Helpers
{
    public static class SlideTracker
    {
        public static int CurrentShowedPresentationSlidePosition
        {
            get
            {
                if (Globals.ThisAddIn.Application.SlideShowWindows.Count <= 0)
                {
                    // currently not in presentation mode
                    return 0;
                }
                return Globals.ThisAddIn.Application.ActivePresentation.SlideShowWindow.View.CurrentShowPosition;
            }
        }

        public static Slide CurrentSlide
        {
            get
            {
                try
                {
                    return Globals.ThisAddIn.Application.ActiveWindow.View.Slide as Slide;
                }
                catch (COMException)
                {
                    // no slide selected or currently in view
                    return null;
                }
            }
        }

        public static void RemoveSlide(int slideId)
        {
            GetSlideById(slideId).Delete();
        }

        public static Tuple<bool, SlideQuestionModel> IsPresentationOnStartArsnovaClickSlide()
        {
            var currentShowedSlidePosition = SlideTracker.CurrentShowedPresentationSlidePosition;

            var slideSessionModel = PresentationInformationStore.GetStoredSlideSessionModel();

            foreach (var slideQuestionModel in slideSessionModel.Questions)
            {
                if (SlideTracker.GetSlideById(slideQuestionModel.ResultsSlideId).SlideNumber == currentShowedSlidePosition)
                    return new Tuple<bool, SlideQuestionModel>(true, slideQuestionModel);
            }

            return new Tuple<bool, SlideQuestionModel>(false, null);
        }

        public static bool IsArsnovaSlide(Slide slide)
        {
            var slideSessionModel = PresentationInformationStore.GetStoredSlideSessionModel();

            return slideSessionModel != null && 
                (slideSessionModel.IntroSlideId == slide.SlideID ||
                slideSessionModel.Questions.Any(questionModel => questionModel.QuestionSlideId == slide.SlideID || questionModel.ResultsSlideId == slide.SlideID));
        }

        public static SlideQuestionModel GetQuestionModelFromSlide(Slide slide)
        {
            var slideSessionModel = PresentationInformationStore.GetStoredSlideSessionModel();

            return slideSessionModel.Questions.First(questionModel => questionModel.QuestionSlideId == slide.SlideID);
        }

        public static Slide GetSlideById(int slideId)
        {
            return Globals.ThisAddIn.Application.ActivePresentation.Slides.FindBySlideID(slideId);
        }
    }
}
