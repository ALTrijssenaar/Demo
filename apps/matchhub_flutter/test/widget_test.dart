import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';

import 'package:matchhub_flutter/main.dart';
import 'package:matchhub_flutter/score_service.dart';

void main() {
  testWidgets('Score increments and shows immediate feedback', (WidgetTester tester) async {
    await tester.pumpWidget(MatchHubApp(scoreService: FakeScoreService()));

    expect(find.byKey(const Key('score-value')), findsOneWidget);
    expect(find.text('0'), findsOneWidget);

    await tester.tap(find.byKey(const Key('add-point-button')));
    await tester.pump();

    expect(find.text('1'), findsOneWidget);
    expect(find.textContaining('Score updated to 1'), findsOneWidget);
  });

  testWidgets('Score never drops below zero', (WidgetTester tester) async {
    await tester.pumpWidget(MatchHubApp(scoreService: FakeScoreService()));

    await tester.tap(find.byKey(const Key('subtract-point-button')));
    await tester.pump();

    expect(find.text('0'), findsOneWidget);
    expect(find.text('Score cannot go below 0'), findsOneWidget);
  });
}
