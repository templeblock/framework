﻿// Accord Unit Tests
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2017
// cesarsouza at gmail.com
//
//    This library is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Lesser General Public
//    License as published by the Free Software Foundation; either
//    version 2.1 of the License, or (at your option) any later version.
//
//    This library is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.
//
//    You should have received a copy of the GNU Lesser General Public
//    License along with this library; if not, write to the Free Software
//    Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
//

namespace Accord.Tests.MachineLearning
{
    using Accord.MachineLearning.DecisionTrees;
    using NUnit.Framework;
    using System;
    using AForge;
    using Accord.Statistics.Filters;
    using System.Data;
    using Accord.Math;
    using System.IO;
    using Accord.IO;
    using Accord.MachineLearning;
    using Accord.MachineLearning.DecisionTrees.Learning;
    using Accord.DataSets;
    using Accord.Math.Optimization.Losses;
    using Accord.MachineLearning.Performance;

    [TestFixture]
    public class DecisionTreeTest
    {


        [Test]
        public void ComputeTest()
        {
            DecisionTree tree;
            int[][] inputs;
            int[] outputs;

            ID3LearningTest.CreateMitchellExample(out tree, out inputs, out outputs);

            Assert.AreEqual(4, tree.InputCount);
            Assert.AreEqual(2, tree.OutputClasses);


            for (int i = 0; i < inputs.Length; i++)
            {
                int y = tree.Compute(inputs[i].ToDouble());
                Assert.AreEqual(outputs[i], y);
            }

        }

        [Test]
        public void EnumerateTest()
        {
            DecisionTree tree;
            int[][] inputs;
            int[] outputs;

            ID3LearningTest.CreateMitchellExample(out tree, out inputs, out outputs);


            DecisionNode[] expected =
            {
                tree.Root,
                tree.Root.Branches[0], // Outlook = 0
                tree.Root.Branches[0].Branches[0], // Humidity = 0
                tree.Root.Branches[0].Branches[1], // Humidity = 1
                tree.Root.Branches[1], // Outlook = 1
                tree.Root.Branches[2], // Outlook = 2
                tree.Root.Branches[2].Branches[0], // Wind = 0
                tree.Root.Branches[2].Branches[1], // Wind = 1
            };

            int i = 0;
            foreach (var node in tree)
            {
                Assert.AreEqual(expected[i++], node);
            }

            Assert.AreEqual(expected.Length, i);
        }

        [Test]
        public void TraverseTest()
        {
            DecisionTree tree;
            int[][] inputs;
            int[] outputs;

            ID3LearningTest.CreateMitchellExample(out tree, out inputs, out outputs);


            {
                DecisionNode[] expected =
                {
                    tree.Root,
                    tree.Root.Branches[0], // Outlook = 0
                    tree.Root.Branches[1], // Outlook = 1
                    tree.Root.Branches[2], // Outlook = 2
                    tree.Root.Branches[0].Branches[0], // Humidity = 0
                    tree.Root.Branches[0].Branches[1], // Humidity = 1
                    tree.Root.Branches[2].Branches[0], // Wind = 0
                    tree.Root.Branches[2].Branches[1], // Wind = 1
                };

                int i = 0;
                foreach (var node in tree.Traverse(DecisionTreeTraversal.BreadthFirst))
                {
                    Assert.AreEqual(expected[i++], node);
                }
                Assert.AreEqual(expected.Length, i);
            }

            {
                DecisionNode[] expected =
                {
                    tree.Root,
                    tree.Root.Branches[0], // Outlook = 0
                    tree.Root.Branches[0].Branches[0], // Humidity = 0
                    tree.Root.Branches[0].Branches[1], // Humidity = 1
                    tree.Root.Branches[1], // Outlook = 1
                    tree.Root.Branches[2], // Outlook = 2
                    tree.Root.Branches[2].Branches[0], // Wind = 0
                    tree.Root.Branches[2].Branches[1], // Wind = 1
                };

                int i = 0;
                foreach (var node in tree.Traverse(DecisionTreeTraversal.DepthFirst))
                {
                    Assert.AreEqual(expected[i++], node);
                }
                Assert.AreEqual(expected.Length, i);
            }

            {
                DecisionNode[] expected =
                {
                    tree.Root.Branches[0].Branches[0], // Humidity = 0
                    tree.Root.Branches[0].Branches[1], // Humidity = 1
                    tree.Root.Branches[0], // Outlook = 0
                    tree.Root.Branches[1], // Outlook = 1
                    tree.Root.Branches[2].Branches[0], // Wind = 0
                    tree.Root.Branches[2].Branches[1], // Wind = 1
                    tree.Root.Branches[2], // Outlook = 2
                    tree.Root,
                };

                int i = 0;
                foreach (var node in tree.Traverse(DecisionTreeTraversal.PostOrder))
                {
                    Assert.AreEqual(expected[i++], node);
                }
                Assert.AreEqual(expected.Length, i);
            }
        }

        //[Test]
        //public void SerializationTest1()
        //{
        //    DecisionTree tree;
        //    int[][] inputs;
        //    int[] outputs;

        //    ID3LearningTest.CreateMitchellExample(out tree, out inputs, out outputs);

        //    Serializer.Save(tree, @"C:\Users\CésarRoberto\Desktop\tree.bin");
        //}

        [Test]
        public void DeserializationTest1()
        {
            MemoryStream stream = new MemoryStream(Properties.Resources.tree);

            DecisionTree tree = Serializer.Load<DecisionTree>(stream);

            Assert.AreEqual(4, tree.InputCount);
            Assert.AreEqual(2, tree.OutputClasses);
            Assert.IsNotNull(tree.Root);

            DecisionTree newtree;
            int[][] inputs;
            int[] outputs;

            ID3LearningTest.CreateMitchellExample(out newtree, out inputs, out outputs);


            for (int i = 0; i < inputs.Length; i++)
            {
                int y = tree.Compute(inputs[i].ToDouble());
                Assert.AreEqual(outputs[i], y);
            }

            DecisionNode[] expected =
            {
                tree.Root,
                tree.Root.Branches[0], // Outlook = 0
                tree.Root.Branches[1], // Outlook = 1
                tree.Root.Branches[2], // Outlook = 2
                tree.Root.Branches[0].Branches[0], // Humidity = 0
                tree.Root.Branches[0].Branches[1], // Humidity = 1
                tree.Root.Branches[2].Branches[0], // Wind = 0
                tree.Root.Branches[2].Branches[1], // Wind = 1
            };

            int c = 0;
            foreach (var node in tree.Traverse(DecisionTreeTraversal.BreadthFirst))
                Assert.AreEqual(expected[c++], node);
            Assert.AreEqual(expected.Length, c);

        }

        [Test]
        public void CrossValidationTest()
        {
            #region doc_cross_validation
            // Ensure we have reproducible results
            Accord.Math.Random.Generator.Seed = 0;

            // Get some data to be learned. We will be using the Wiconsin's
            // (Diagnostic) Breast Cancer dataset, where the goal is to determine
            // whether the characteristics extracted from a breast cancer exam
            // correspond to a malignant or benign type of cancer:
            var data = new WisconsinDiagnosticBreastCancer();
            double[][] input = data.Features; // 569 samples, 30-dimensional features
            int[] output = data.ClassLabels;  // 569 samples, 2 different class labels

            // Let's say we want to measure the cross-validation performance of
            // a decision tree with a maximum tree height of 5 and where variables
            // are able to join the decision path at most 2 times during evaluation:
            var cv = CrossValidation.Create(

                k: 10, // We will be using 10-fold cross validation

                learner: (p) => new C45Learning() // here we create the learning algorithm
                {
                    Join = 2,
                    MaxHeight = 5
                },

                // Now we have to specify how the tree performance should be measured:
                loss: (actual, expected, p) => new ZeroOneLoss(expected).Loss(actual),

                // This function can be used to perform any special
                // operations before the actual learning is done, but
                // here we will just leave it as simple as it can be:
                fit: (teacher, x, y, w) => teacher.Learn(x, y, w),

                // Finally, we have to pass the input and output data
                // that will be used in cross-validation. 
                x: input, y: output
            );

            // After the cross-validation object has been created,
            // we can call its .Learn method with the input and 
            // output data that will be partitioned into the folds:
            var result = cv.Learn(input, output);

            // We can grab some information about the problem:
            int numberOfSamples = result.NumberOfSamples; // should be 569
            int numberOfInputs = result.NumberOfInputs;   // should be 30
            int numberOfOutputs = result.NumberOfOutputs; // should be 2

            double trainingError = result.Training.Mean; // should be 0
            double validationError = result.Validation.Mean; // should be 0.089661654135338359
            #endregion

            Assert.AreEqual(569, numberOfSamples);
            Assert.AreEqual(30, numberOfInputs);
            Assert.AreEqual(2, numberOfOutputs);

            Assert.AreEqual(10, cv.K);
            Assert.AreEqual(0.017770391691033137, result.Training.Mean, 1e-10);
            Assert.AreEqual(0.077318295739348369, result.Validation.Mean, 1e-10);

            Assert.AreEqual(3.0913682243756776E-05, result.Training.Variance, 1e-10);
            Assert.AreEqual(0.00090104473101439207, result.Validation.Variance, 1e-10);

            Assert.AreEqual(10, cv.Folds.Length);
            Assert.AreEqual(10, result.Models.Length);

            var tree = result.Models[0].Model;
            int height = tree.GetHeight();
            Assert.AreEqual(5, height);

            cv = CrossValidation.Create(
               k: 10,
               learner: (p) => new C45Learning()
               {
                   Join = 1,
                   MaxHeight = 1,
                   MaxVariables = 1
               },
               loss: (actual, expected, p) => new ZeroOneLoss(expected).Loss(actual),
               fit: (teacher, x, y, w) => teacher.Learn(x, y, w),
               x: input, y: output
            );

            result = cv.Learn(input, output);

            tree = result.Models[0].Model;
            height = tree.GetHeight();

            Assert.AreEqual(1, height);

            Assert.AreEqual(0.10896305433723197, result.Training.Mean, 5e-3);
            Assert.AreEqual(0.1125, result.Validation.Mean, 1e-10);

            Assert.AreEqual(2.1009258672955873E-05, result.Training.Variance, 1e-10);
            Assert.AreEqual(0.0017292179645018977, result.Validation.Variance, 1e-10);
        }
    }
}
