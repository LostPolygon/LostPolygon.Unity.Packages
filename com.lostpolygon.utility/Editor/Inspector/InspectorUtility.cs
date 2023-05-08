using System;
using System.Collections.Generic;
using UnityEditor;

namespace LostPolygon.Unity.Utility.Editor {
    /// <summary>
    /// Helper for drawing multi-object fields.
    /// </summary>
    public static class InspectorUtility<TContainer> {
        public delegate bool ExtraChangeCheckFunc<in TField>(TField oldValue, TField newValue, bool isValueMixed);

        /// <summary>
        /// Checks whether the field values are different and returns the
        /// field value of the last element.
        /// </summary>
        /// <param name="dataContainers">
        /// The data containers.
        /// </param>
        /// <param name="valueGetterFunc">
        /// The function that retrieves a specific field from <typeparamref name="TContainer"/>.
        /// </param>
        /// <param name="value">
        /// The field value of the last <paramref name="dataContainers"/> element.
        /// </param>
        /// <returns>
        /// True if field values are different, false otherwise.
        /// </returns>
        /// <typeparam name="TField">The data container type.</typeparam>
        public static bool IsMixedValues<TField>(
            IEnumerable<TContainer> dataContainers,
            Func<TContainer, TField> valueGetterFunc,
            out TField value
        ) {
            value = default;
            bool isValueSet = false;
            bool isValueMixed = false;

            foreach (TContainer dataContainer in dataContainers) {
                if (isValueSet && !EqualityComparer<TField>.Default.Equals(value, valueGetterFunc(dataContainer))) {
                    isValueMixed = true;
                }

                value = valueGetterFunc(dataContainer);
                isValueSet = true;
            }

            return isValueMixed;
        }

        /// <summary>
        /// Draws a single field.
        /// </summary>
        /// <param name="dataContainers">
        /// The data containers.
        /// </param>
        /// <param name="valueGetterFunc">
        /// The function that retrieves a specific field from <typeparamref name="TContainer"/>.
        /// </param>
        /// <param name="valueSetterAction">
        /// The field value of the last <paramref name="dataContainers"/> element.
        /// </param>
        /// <param name="drawerFunc">
        /// The function that draws a GUI for the field.
        /// </param>
        /// <returns>
        /// True if GUI was changed, false otherwise.
        /// </returns>
        /// <typeparam name="TField">The data container type.</typeparam>
        public static bool DrawField<TField>(
            TContainer[] dataContainers,
            Func<TContainer, TField> valueGetterFunc,
            Action<int, TContainer, TField> valueSetterAction,
            Func<TField, TField> drawerFunc
        ) {
            return DrawField(dataContainers, valueGetterFunc, valueSetterAction, drawerFunc, out TField _);
        }

        /// <summary>
        /// Draws a single field.
        /// </summary>
        /// <param name="dataContainers">
        /// The data containers.
        /// </param>
        /// <param name="valueGetterFunc">
        /// The function that retrieves a specific field from <typeparamref name="TContainer"/>.
        /// </param>
        /// <param name="valueSetterAction">
        /// The field value of the last <paramref name="dataContainers"/> element.
        /// </param>
        /// <param name="drawerFunc">
        /// The function that draws a GUI for the field.
        /// </param>
        /// <param name="newValue">
        /// The new value.
        /// </param>
        /// <returns>
        /// True if GUI was changed, false otherwise.
        /// </returns>
        /// <typeparam name="TField">The data container type.</typeparam>
        public static bool DrawField<TField>(
            TContainer[] dataContainers,
            Func<TContainer, TField> valueGetterFunc,
            Action<int, TContainer, TField> valueSetterAction,
            Func<TField, TField> drawerFunc,
            out TField newValue,
            ExtraChangeCheckFunc<TField> extraChangeCheck = null
        ) {
            bool isValueMixed = IsMixedValues(dataContainers, valueGetterFunc, out TField oldValue);

            if (isValueMixed) {
                EditorGUI.showMixedValue = true;
            }

            EditorGUI.BeginChangeCheck();
            newValue = drawerFunc(oldValue);
            bool isChanged = EditorGUI.EndChangeCheck();
            if (!isChanged && extraChangeCheck != null) {
                isChanged |= extraChangeCheck.Invoke(oldValue, newValue, isValueMixed);
            }

            if (isChanged) {
                for (int i = 0; i < dataContainers.Length; i++) {
                    TContainer settings = dataContainers[i];
                    valueSetterAction(i, settings, newValue);
                }
            }

            EditorGUI.showMixedValue = false;

            return isChanged;
        }
    }
}
