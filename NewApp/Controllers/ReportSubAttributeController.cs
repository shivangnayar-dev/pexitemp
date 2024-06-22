using Microsoft.AspNetCore.Mvc;
using NewApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NewApp.Controllers
{
    [Route("api/[controller]")]
    public class ReportSubAttributeController : Controller
    {
        private readonly CandidateDbContext _context;

        public ReportSubAttributeController(CandidateDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("CheckreportIdValidity")]
        public IActionResult CheckreportIdValidity([FromBody] ReportSubAttribute reportSubAttribute)
        {
            bool isValid = CheckTestCodeValidityInDatabase(reportSubAttribute.ReportId);
            Dictionary<string, Dictionary<string, Dictionary<int, List<object>>>> assessmentSubAttributesData = GetAssessmentSubAttributesBy(reportSubAttribute.ReportId);

            // Randomly select questions along with their sub-attributes data
            var selectedQuestionsWithSubAttributes = CombineAndReturnIdsAndQuestions(assessmentSubAttributesData);
            var GetOptionsAndAnswerIdsByAssessmentSubAttributee = GetOptionsAndAnswerIdsByAssessmentSubAttribute(selectedQuestionsWithSubAttributes);
            // Combine selected questions
            var optionsAndAnswerIds = GetOptionsAndAnswerIdsWithQuestions(selectedQuestionsWithSubAttributes);

            var questionTextList = FetchQuestionData(selectedQuestionsWithSubAttributes.Select(sq => sq.QuestionId).ToList());

            // Fetch AssessmentSubAttribute, CountofQuestiontoDisplay, and QuestionCount
            var assessmentSubAttributes = selectedQuestionsWithSubAttributes.Select(sq =>
            {
                var countOfQuestionToDisplay = sq.SubAttributesData.Values.Last().Keys.Last();
                var questionCount = sq.SubAttributesData.Values.Last().Values.Last()[0]; // Assuming there's at least one question in the subAttribute
               var assessmentSubAttributeName = sq.SubAttributesData.Values.Last().Values.Last();
                return new
                {
                    AssessmentSubAttribute = sq.SubAttributesData.Keys.Last(),
                    CountofQuestiontoDisplay = countOfQuestionToDisplay,
                    creativityValue = assessmentSubAttributeName[3],


                QuestionCount = questionCount,
                    SelectedQuestionsCount = 1, // Each selected question represents one count
                    SelectedQuestions = new Dictionary<string, string> { { sq.QuestionId.ToString(), sq.Question } }
                };
            }).ToList();

            // Modify the structure to match the expected format
            var formattedOptionsAndAnswerIds = optionsAndAnswerIds.ToDictionary(
    kvp => kvp.Key.ToString(), // Assuming kvp.Key is the question ID
    kvp => new
    {
        question_id = kvp.Key.ToString(), // Assigning kvp.Key as question_id
        question = kvp.Value.Question,
        optionsAndAnswerIds = kvp.Value.OptionsAndAnswerIds.Select(opt => new
        {
            Item1 = opt.Options,
            Item2 = opt.AnswerId,
            Item3 = opt.SequenceOfDisplay,
            Item4 = opt.MarksTotal
        }).ToList(),
        SubAttributeId = kvp.Value.SubAttributeId, // Using SubAttributeId from questionOptionsAndAnswers
        AssessmentSubAttribute = kvp.Value.AssessmentSubAttribute
        // Get the first key or default value if not found
    });
            var formattedOptionsAndAnswerIdss = GetOptionsAndAnswerIdsByAssessmentSubAttributee.ToDictionary(
               kvp => kvp.Key.ToString(), // kvp.Key is the assessmentSubAttribute
               kvp => new
               {
                   assessmentSubAttribute = kvp.Key, // Using kvp.Key as AssessmentSubAttribute
                   questions = kvp.Value.Select(q => new
                   {
                       question_id = q.QuestionId.ToString(),
                       question = q.Question,
                       optionsAndAnswerIds = q.OptionsAndAnswerIds.Select(opt => new
                       {
                           Item1 = opt.Options,
                           Item2 = opt.AnswerId,
                           Item3 = opt.SequenceOfDisplay,
                           Item4 = opt.MarksTotal
                       }).ToList(),
                       SubAttributeId = q.SubAttributeId,
                   }).ToList()
               });




            // Check if the sum of countToDisplay of all attributes is equal to the length of combinedQuestions
            int sumCountToDisplay = assessmentSubAttributesData.Values
                .SelectMany(subAttributeData => subAttributeData.Values)
                .Sum(questionData => questionData.Keys.First());

            bool isCountMatch = sumCountToDisplay == selectedQuestionsWithSubAttributes.Count;

            return Json(new
            {
                isValid = isValid,
                assessmentSubAttributes = assessmentSubAttributes,
                questionOptionsAndAnswers = formattedOptionsAndAnswerIds,
                isCountMatch = isCountMatch,
                sumCountToDisplay = sumCountToDisplay,
                questionOptionsAndAnswerss = formattedOptionsAndAnswerIdss,

                selectedQuestionsWithSubAttributes = selectedQuestionsWithSubAttributes.Select(sq => new
                {
                    QuestionId = sq.QuestionId,
                    Question = sq.Question,
                  
                    SubAttributeId = sq.SubAttributeId
             
                })
            });
        }

        private Dictionary<Guid, (Guid QuestionId, string Question, Dictionary<string, Dictionary<int, List<object>>> SubAttributesData, string SubAttributeId, string AssessmentSubAttribute, List<(string Options, string AnswerId, int SequenceOfDisplay, int MarksTotal)> OptionsAndAnswerIds)> GetOptionsAndAnswerIdsWithQuestions(List<(Guid QuestionId, string Question, Dictionary<string, Dictionary<int, List<object>>> SubAttributesData, string SubAttributeId, string AssessmentSubAttribute)> selectedQuestions)
        {
            Dictionary<Guid, (Guid QuestionId, string Question, Dictionary<string, Dictionary<int, List<object>>> SubAttributesData, string SubAttributeId, string AssessmentSubAttribute, List<(string Options, string AnswerId, int SequenceOfDisplay, int MarksTotal)> OptionsAndAnswerIds)> questionOptionsAndAnswers = new Dictionary<Guid, (Guid QuestionId, string Question, Dictionary<string, Dictionary<int, List<object>>> SubAttributesData, string SubAttributeId, string AssessmentSubAttribute, List<(string Options, string AnswerId, int SequenceOfDisplay, int MarksTotal)>)>();

            using (var dbContext = new QuestionAnsMapDbContext())
            {
                foreach (var selectedQuestion in selectedQuestions)
                {
                    var questionId = selectedQuestion.QuestionId;
                    var subAttributeId = selectedQuestion.SubAttributeId;
                    var assessmentSubAttribute = selectedQuestion.AssessmentSubAttribute;

                    // Ensure that the Select statement returns the correct types
                    var questionDataList = _context.QuestionAnsMaps
                        .Where(q => q.QuestionId == questionId)
                        .Select(q => new
                        {
                            q.QuestionId,
                            q.Question,
                            q.Options,
                            q.AnswerId,
                            q.SequenceOfDisplay,
                            q.MarksTotal
                        })
                        .OrderBy(q => q.SequenceOfDisplay)
                        .ToList();

                    if (questionDataList.Any())
                    {
                        // Use the correct property names and types when adding to the dictionary
                        var optionsAndAnswerList = questionDataList
                            .Select(q => (
                                Options: q.Options,
                                AnswerId: q.AnswerId,
                                SequenceOfDisplay: q.SequenceOfDisplay,
                                MarksTotal: q.MarksTotal))
                            .ToList();

                        var questionAndOptions = (
                            QuestionId: questionDataList.First().QuestionId,
                            Question: questionDataList.First().Question,
                            SubAttributesData: selectedQuestion.SubAttributesData,
                            SubAttributeId: subAttributeId,
                            AssessmentSubAttribute: assessmentSubAttribute,
                            OptionsAndAnswerIds: optionsAndAnswerList);


                        questionOptionsAndAnswers.Add(questionId, questionAndOptions);
                    }
                }
            }

            return questionOptionsAndAnswers;
        }
        private Dictionary<string, List<(Guid QuestionId, string Question, Dictionary<string, Dictionary<int, List<object>>> SubAttributesData, string SubAttributeId, string AssessmentSubAttribute, List<(string Options, string AnswerId, int SequenceOfDisplay, int MarksTotal)> OptionsAndAnswerIds)>> GetOptionsAndAnswerIdsByAssessmentSubAttribute(List<(Guid QuestionId, string Question, Dictionary<string, Dictionary<int, List<object>>> SubAttributesData, string SubAttributeId, string AssessmentSubAttribute)> selectedQuestions)
        {
            Dictionary<string, List<(Guid QuestionId, string Question, Dictionary<string, Dictionary<int, List<object>>> SubAttributesData, string SubAttributeId, string AssessmentSubAttribute, List<(string Options, string AnswerId, int SequenceOfDisplay, int MarksTotal)> OptionsAndAnswerIds)>> questionOptionsByAssessmentSubAttribute = new Dictionary<string, List<(Guid QuestionId, string Question, Dictionary<string, Dictionary<int, List<object>>> SubAttributesData, string SubAttributeId, string AssessmentSubAttribute, List<(string Options, string AnswerId, int SequenceOfDisplay, int MarksTotal)> OptionsAndAnswerIds)>>();

            foreach (var selectedQuestion in selectedQuestions)
            {
                var questionId = selectedQuestion.QuestionId;
                var subAttributeId = selectedQuestion.SubAttributeId;
                var assessmentSubAttribute = selectedQuestion.AssessmentSubAttribute;

                // Ensure that the Select statement returns the correct types
                var questionDataList = _context.QuestionAnsMaps
                    .Where(q => q.QuestionId == questionId)
                    .Select(q => new
                    {
                        q.QuestionId,
                        q.Question,
                        q.Options,
                        q.AnswerId,
                        q.SequenceOfDisplay,
                        q.MarksTotal
                    })
                    .OrderBy(q => q.SequenceOfDisplay)
                    .ToList();

                if (questionDataList.Any())
                {
                    // Use the correct property names and types when adding to the dictionary
                    var optionsAndAnswerList = questionDataList
                        .Select(q => (
                            Options: q.Options,
                            AnswerId: q.AnswerId,
                            SequenceOfDisplay: q.SequenceOfDisplay,
                            MarksTotal: q.MarksTotal))
                        .ToList();

                    var questionAndOptions = (
                        QuestionId: questionDataList.First().QuestionId,
                        Question: questionDataList.First().Question,
                        SubAttributesData: selectedQuestion.SubAttributesData,
                        SubAttributeId: subAttributeId,
                        AssessmentSubAttribute: assessmentSubAttribute,
                        OptionsAndAnswerIds: optionsAndAnswerList);

                    if (!questionOptionsByAssessmentSubAttribute.ContainsKey(assessmentSubAttribute))
                    {
                        questionOptionsByAssessmentSubAttribute[assessmentSubAttribute] = new List<(Guid QuestionId, string Question, Dictionary<string, Dictionary<int, List<object>>> SubAttributesData, string SubAttributeId, string AssessmentSubAttribute, List<(string Options, string AnswerId, int SequenceOfDisplay, int MarksTotal)> OptionsAndAnswerIds)>();
                    }

                    questionOptionsByAssessmentSubAttribute[assessmentSubAttribute].Add(questionAndOptions);
                }
            }

            return questionOptionsByAssessmentSubAttribute;
        }


        private List<string> FetchQuestionData(List<Guid> selectedQuestionIds)
        {
            List<string> questionTextList = new List<string>();

            using (var dbContext = new QuestionAnsMapDbContext())
            {
                foreach (var questionId in selectedQuestionIds)
                {
                    var questionText = _context.QuestionAnsMaps
                        .Where(q => q.QuestionId == questionId)
                        .Select(q => q.Question)
                        .FirstOrDefault();

                    if (!string.IsNullOrEmpty(questionText))
                    {
                        questionTextList.Add(questionText);
                    }
                }
            }

            return questionTextList;
        }
      

        private Dictionary<string, Dictionary<string, Dictionary<int, List<object>>>> GetAssessmentSubAttributesBy(string ReportId)
        {
            var distinctAssessmentSubAttributeIds = _context.ReportSubAttributes
                .Where(tc => tc.ReportId == ReportId)
                .Select(tc => tc.AssessmentSubAttributeId)
                .ToList();

            Dictionary<string, Dictionary<string, Dictionary<int, List<object>>>> assessmentSubAttributesData = new Dictionary<string, Dictionary<string, Dictionary<int, List<object>>>>();

            foreach (var assessmentSubAttributeId in distinctAssessmentSubAttributeIds)
            {
                var values = _context.ReportSubAttributes
                    .Where(tc => tc.ReportId == ReportId && tc.AssessmentSubAttributeId == assessmentSubAttributeId)
                    .Select(tc => new
                    {
                        tc.AssessmentSubAttributeId,
                        tc.AssessmentSubAttribute,
                        tc.CountofQuestiontoDisplay,
                        tc.QuestionCount,
                        tc.TimeperQuestioninSec,
                        tc.GradeCategory
                    })
                    .ToList();

                Dictionary<string, Dictionary<int, List<object>>> subAttributeData = new Dictionary<string, Dictionary<int, List<object>>>();
                int cumulativeCount = 0;

                foreach (var value in values)
                {
                    cumulativeCount += value.CountofQuestiontoDisplay;

                    if (!subAttributeData.ContainsKey(value.AssessmentSubAttributeId))
                    {
                        subAttributeData.Add(value.AssessmentSubAttributeId, new Dictionary<int, List<object>>());
                    }

                    var questions = _context.TotalQuestions
                        .Where(tq => tq.AssessmentSubAttributeId == value.AssessmentSubAttributeId)
                        .ToDictionary(tq => tq.QuestionId, tq => tq.Question);

                    subAttributeData[value.AssessmentSubAttributeId].Add(cumulativeCount, new List<object> { value.QuestionCount, value.TimeperQuestioninSec, questions,value.AssessmentSubAttribute,value.GradeCategory });
                }

                assessmentSubAttributesData.Add(assessmentSubAttributeId, subAttributeData);
            }

            return assessmentSubAttributesData;
        }


        private bool CheckTestCodeValidityInDatabase(string ReportId)
        {
            return _context.ReportSubAttributes.Any(tc => tc.ReportId == ReportId);
        }


        private List<(Guid QuestionId, string Question, Dictionary<string, Dictionary<int, List<object>>> SubAttributesData, string SubAttributeId, string AssessmentSubAttribute)> CombineAndReturnIdsAndQuestions(Dictionary<string, Dictionary<string, Dictionary<int, List<object>>>> assessmentSubAttributesData)
        {
            List<(Guid QuestionId, string Question, Dictionary<string, Dictionary<int, List<object>>> SubAttributesData, string SubAttributeId, string AssessmentSubAttribute)> selectedQuestionsWithSubAttributes = new List<(Guid QuestionId, string Question, Dictionary<string, Dictionary<int, List<object>>> SubAttributesData, string SubAttributeId, string AssessmentSubAttribute)>();

            foreach (var subAttributeData in assessmentSubAttributesData)
            {
                var subAttributeId = subAttributeData.Key; // Get the subAttributeId

                foreach (var questionData in subAttributeData.Value)
                {
                    int countToDisplay = questionData.Value.Keys.First();

                    if (questionData.Value.ContainsKey(countToDisplay))
                    {
                        List<object> questionsList = questionData.Value[countToDisplay];

                        if (questionsList.Count > 2 && questionsList[2] is Dictionary<Guid, string> subSelectedQuestions)
                        {
                            // Shuffle the questions
                            var shuffledQuestions = subSelectedQuestions.OrderBy(x => Guid.NewGuid()).ToDictionary(pair => pair.Key, pair => pair.Value);

                            // Select a subset of questions based on countToDisplay
                            var selectedSubset = shuffledQuestions.Take(countToDisplay).ToDictionary(pair => pair.Key, pair => pair.Value);

                            foreach (var question in selectedSubset)
                            {
                                // Fetch AssessmentSubAttribute from subAttributesData
                                string assessmentSubAttribute = (string)questionData.Value.Last().Value.Last(); // Assuming the AssessmentSubAttribute is stored at index 3

                                // Create the SubAttributesData dictionary containing the subAttributeId and its data
                                var subAttributesData = new Dictionary<string, Dictionary<int, List<object>>>();
                                subAttributesData.Add(subAttributeId, questionData.Value);

                                selectedQuestionsWithSubAttributes.Add((
                                    question.Key,
                                    question.Value,
                                    subAttributesData,
                                    subAttributeId,
                                    assessmentSubAttribute
                                ));
                            }
                        }
                    }
                }
            }

            return selectedQuestionsWithSubAttributes;
        }


    }
}

