﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace TranceSql.Language
{
    /// <summary>
    /// Represents a JOIN clause in a <see cref="Select"/> statement.
    /// </summary>
    public class Join : ISqlElement
    {
        /// <summary>
        /// Gets the join type.
        /// </summary>
        private JoinType JoinType { get; }

        /// <summary>
        /// Gets or sets the table.
        /// </summary>
        private Table Table { get; set; }

        private ConditionCollection _on;
        /// <summary>
        /// Gets or sets the join condition.
        /// </summary>
        public ConditionCollection On
        {
            get => _on = _on ?? new ConditionCollection();
            set => _on = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Join"/> class.
        /// </summary>
        /// <param name="joinType">Type of the join to create.</param>
        public Join(JoinType joinType)
        {
            JoinType = joinType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Join"/> class.
        /// </summary>
        /// <param name="joinType">Type of the join to create.</param>
        /// <param name="table">The table to join.</param>
        /// <param name="on">The join condition.</param>
        public Join(JoinType joinType, Table table, ICondition on)
        {
            JoinType = joinType;
            Table = table;
            On.Add(on);
        }

        void ISqlElement.Render(RenderContext context)
        {
            switch (JoinType)
            {
                case JoinType.Join:
                    context.Write("\nJOIN ");
                    break;
                case JoinType.Inner:
                    context.Write("\nINNER JOIN ");
                    break;
                case JoinType.LeftOuter:
                    context.Write("\nLEFT JOIN ");
                    break;
                case JoinType.RightOuter:
                    context.Write("\nRIGHT JOIN ");
                    break;
                case JoinType.FullOuter:
                    context.Write("\nOUT JOIN ");
                    break;
                case JoinType.Cross:
                    context.Write("\nCROSS JOIN ");
                    break;
                default:
                    break;
            }

            context.Render(Table);

            if (On.Any())
            {
                context.Write(" ON ");
                On.RenderCollection(context);
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString() => this.RenderDebug();
    }
}